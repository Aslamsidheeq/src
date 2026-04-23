using FieldOps.Domain.Entities.Master;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(MasterDbContext dbContext)
{
    public Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken) =>
        dbContext.Tenants.FirstOrDefaultAsync(x => x.Subdomain == subdomain && x.IsActive, cancellationToken);

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return tenant;
    }
}
