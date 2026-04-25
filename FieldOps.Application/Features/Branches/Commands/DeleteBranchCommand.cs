using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Branches.Commands;

public sealed record DeleteBranchCommand(int Id) : IRequest<Result<bool>>;

public sealed class DeleteBranchCommandHandler(ITenantDbContext tenantDbContext)
    : IRequestHandler<DeleteBranchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await tenantDbContext.Branches.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (branch is null)
        {
            return Result<bool>.Fail("Branch not found.", ErrorCodes.BranchNotFound);
        }

        var hasActiveUsers = await tenantDbContext.Users
            .AnyAsync(x => x.BranchId == request.Id && x.IsActive && !x.IsDeleted, cancellationToken);
        if (hasActiveUsers)
        {
            return Result<bool>.Fail("Branch has active users and cannot be deleted.", ErrorCodes.BranchHasActiveUsers);
        }

        var hasActiveWorkOrders = await tenantDbContext.WorkOrders
            .AnyAsync(x => x.BranchId == request.Id && !x.IsDeleted, cancellationToken);
        if (hasActiveWorkOrders)
        {
            return Result<bool>.Fail("Branch has active work orders and cannot be deleted.", ErrorCodes.BranchHasActiveWorkOrders);
        }

        branch.IsDeleted = true;
        branch.DeletedAtUtc = DateTime.UtcNow;
        branch.UpdatedAtUtc = DateTime.UtcNow;
        await tenantDbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}

