using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Branches.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Branches.Queries;

public sealed record GetBranchByIdQuery(int Id) : IRequest<Result<BranchDto>>;

public sealed class GetBranchByIdQueryHandler(
    ITenantDbContext tenantDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetBranchByIdQuery, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branchQuery = tenantDbContext.Branches
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id);

        if (!currentUserService.IsTenantAdmin)
        {
            if (currentUserService.BranchId is null)
            {
                return Result<BranchDto>.Fail("User is not assigned to a branch.", ErrorCodes.Unauthorized);
            }

            branchQuery = branchQuery.Where(x => x.Id == currentUserService.BranchId.Value);
        }

        var result = await (
            from b in branchQuery
            join u in tenantDbContext.Users.AsNoTracking() on b.ManagerId equals (int?)u.Id into mgr
            from m in mgr.DefaultIfEmpty()
            select new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                Emirate = b.Emirate.ToString(),
                Address = b.Address,
                Trn = b.Trn,
                Iban = b.Iban,
                InvoicePrefix = b.InvoicePrefix,
                ManagerId = b.ManagerId,
                ManagerName = m == null ? null : m.Email,
                CreatedAtUtc = b.CreatedAtUtc,
                IsActive = true
            }).FirstOrDefaultAsync(cancellationToken);

        return result is null
            ? Result<BranchDto>.Fail("Branch not found.", ErrorCodes.BranchNotFound)
            : Result<BranchDto>.Ok(result);
    }
}

