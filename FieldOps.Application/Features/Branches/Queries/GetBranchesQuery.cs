using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Branches.DTOs;
using FieldOps.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Branches.Queries;

public sealed record GetBranchesQuery(int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PagedResult<BranchDto>>>;

public sealed class GetBranchesQueryHandler(
    ITenantDbContext tenantDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetBranchesQuery, Result<PagedResult<BranchDto>>>
{
    public async Task<Result<PagedResult<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var branchesQuery = tenantDbContext.Branches
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!currentUserService.IsTenantAdmin)
        {
            if (currentUserService.BranchId is null)
            {
                return Result<PagedResult<BranchDto>>.Fail("User is not assigned to a branch.", ErrorCodes.Unauthorized);
            }

            branchesQuery = branchesQuery.Where(x => x.Id == currentUserService.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            branchesQuery = branchesQuery.Where(x => x.Name.Contains(term));
        }

        var total = await branchesQuery.LongCountAsync(cancellationToken);

        var items = await branchesQuery
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BranchDto
            {
                Id = x.Id,
                Name = x.Name,
                Emirate = x.Emirate.ToString(),
                Address = x.Address,
                Trn = x.Trn,
                Iban = x.Iban,
                InvoicePrefix = x.InvoicePrefix,
                ManagerId = x.ManagerId,
                ManagerName = null,
                CreatedAtUtc = x.CreatedAtUtc,
                IsActive = true
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<BranchDto>>.Ok(new PagedResult<BranchDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        });
    }
}

