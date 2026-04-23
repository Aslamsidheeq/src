using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Domain.Entities.Master;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Tenants;

public sealed record TenantDto(Guid Id, string Name, string Subdomain, bool IsActive);
public sealed record CreateTenantRequest(string Name, string Subdomain, string Trn, string ContactEmail, string AdminPassword);

public sealed record CreateTenantCommand(CreateTenantRequest Request) : IRequest<Result<TenantDto>>;
public sealed record GetAllTenantsQuery() : IRequest<Result<PagedResult<TenantDto>>>;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Subdomain).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.Request.Trn).NotEmpty();
        RuleFor(x => x.Request.ContactEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.AdminPassword).NotEmpty().MinimumLength(8);
    }
}

public sealed class CreateTenantCommandHandler(
    IMasterDbContext masterDbContext,
    IPasswordService passwordService,
    ITenantProvisioningService tenantProvisioningService) : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var normalizedSubdomain = request.Request.Subdomain.Trim().ToLowerInvariant();
        var exists = await masterDbContext.Tenants.AnyAsync(x => x.Subdomain == normalizedSubdomain, cancellationToken);
        if (exists)
        {
            return Result<TenantDto>.Fail("A tenant with this subdomain already exists.", ErrorCodes.TenantAlreadyExists);
        }

        var tenant = new Tenant
        {
            Name = request.Request.Name.Trim(),
            Subdomain = normalizedSubdomain,
            Trn = request.Request.Trn.Trim(),
            DatabaseName = $"{normalizedSubdomain.Replace("-", "_")}_db",
            IsActive = true
        };

        var passwordHash = passwordService.Hash(request.Request.AdminPassword);
        await tenantProvisioningService.ProvisionAsync(tenant, request.Request.ContactEmail.Trim(), passwordHash, cancellationToken);

        return Result<TenantDto>.Ok(new TenantDto(tenant.Id, tenant.Name, tenant.Subdomain, tenant.IsActive));
    }
}

public sealed class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, Result<PagedResult<TenantDto>>>
{
    public Task<Result<PagedResult<TenantDto>>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result<PagedResult<TenantDto>>.Ok(new PagedResult<TenantDto>
        {
            Items = Array.Empty<TenantDto>(),
            Page = 1,
            PageSize = 20,
            Total = 0
        }));
}
