using FieldOps.Domain.Entities.Master;

namespace FieldOps.Application.Common.Interfaces;

public interface ITenantProvisioningService
{
    Task ProvisionAsync(Tenant tenant, string adminEmail, string adminPasswordHash, CancellationToken cancellationToken);
}
