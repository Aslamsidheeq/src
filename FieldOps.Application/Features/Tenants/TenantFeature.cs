using FieldOps.Application.Common.Models;
using MediatR;

namespace FieldOps.Application.Features.Tenants;

public sealed record TenantDto(Guid Id, string Name, string Subdomain, bool IsActive);
public sealed record CreateTenantRequest(string Name, string Subdomain, string Trn, string ContactEmail);

public sealed record CreateTenantCommand(CreateTenantRequest Request) : IRequest<Result<TenantDto>>;
public sealed record GetAllTenantsQuery() : IRequest<Result<PagedResult<TenantDto>>>;

public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    public Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var dto = new TenantDto(Guid.NewGuid(), request.Request.Name, request.Request.Subdomain, true);
        return Task.FromResult(Result<TenantDto>.Ok(dto));
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
