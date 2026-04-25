using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Branches.DTOs;
using FieldOps.Domain.Entities.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Branches.Commands;

public sealed record CreateBranchCommand(CreateBranchRequest Request) : IRequest<Result<BranchDto>>;

public sealed class CreateBranchCommandHandler(ITenantDbContext tenantDbContext)
    : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = new Branch
        {
            Name = request.Request.Name.Trim(),
            Emirate = request.Request.Emirate,
            Address = request.Request.Address.Trim(),
            Trn = request.Request.Trn?.Trim() ?? string.Empty,
            Iban = request.Request.Iban?.Trim() ?? string.Empty,
            InvoicePrefix = (request.Request.InvoicePrefix ?? string.Empty).Trim().ToUpperInvariant(),
            ManagerId = request.Request.ManagerId
        };

        tenantDbContext.Branches.Add(branch);
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

