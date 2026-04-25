using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Branches.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Branches.Commands;

public sealed record UpdateBranchCommand(int Id, UpdateBranchRequest Request) : IRequest<Result<BranchDto>>;

public sealed class UpdateBranchCommandHandler(ITenantDbContext tenantDbContext)
    : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await tenantDbContext.Branches.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (branch is null)
        {
            return Result<BranchDto>.Fail("Branch not found.", ErrorCodes.BranchNotFound);
        }

        branch.Name = request.Request.Name.Trim();
        branch.Emirate = request.Request.Emirate;
        branch.Address = request.Request.Address.Trim();
        branch.Trn = request.Request.Trn?.Trim() ?? string.Empty;
        branch.Iban = request.Request.Iban?.Trim() ?? string.Empty;
        branch.InvoicePrefix = (request.Request.InvoicePrefix ?? string.Empty).Trim().ToUpperInvariant();
        branch.ManagerId = request.Request.ManagerId;
        branch.UpdatedAtUtc = DateTime.UtcNow;

        await tenantDbContext.SaveChangesAsync(cancellationToken);

        string? managerName = null;
        if (branch.ManagerId.HasValue)
        {
            managerName = await tenantDbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == branch.ManagerId.Value)
                .Select(x => x.Email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return Result<BranchDto>.Ok(new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Emirate = branch.Emirate.ToString(),
            Address = branch.Address,
            Trn = branch.Trn,
            Iban = branch.Iban,
            InvoicePrefix = branch.InvoicePrefix,
            ManagerId = branch.ManagerId,
            ManagerName = managerName,
            CreatedAtUtc = branch.CreatedAtUtc,
            IsActive = true
        });
    }
}

