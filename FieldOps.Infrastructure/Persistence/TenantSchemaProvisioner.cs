using FieldOps.Domain.Entities.Master;
using FieldOps.Domain.Entities.Tenant;
using FieldOps.Domain.Enums;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Infrastructure.Persistence;

public sealed class TenantSchemaProvisioner(MasterDbContext masterDbContext, TenantDbContext tenantDbContext)
{
    public async Task ProvisionAsync(Tenant tenant, string adminEmail, string adminPasswordHash, CancellationToken cancellationToken)
    {
        masterDbContext.Tenants.Add(tenant);
        masterDbContext.Subscriptions.Add(new Subscription
        {
            TenantId = tenant.Id,
            Plan = tenant.Plan,
            StartedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddYears(1),
            IsActive = true
        });
        await masterDbContext.SaveChangesAsync(cancellationToken);

        await tenantDbContext.Database.EnsureCreatedAsync(cancellationToken);

        var mainBranch = new Branch
        {
            Name = "Main Branch",
            Emirate = Emirates.Dubai,
            Address = "Dubai",
            InvoicePrefix = "INV"
        };
        tenantDbContext.Branches.Add(mainBranch);

        tenantDbContext.Users.Add(new User
        {
            BranchId = mainBranch.Id,
            Email = adminEmail,
            PasswordHash = adminPasswordHash,
            Role = UserRole.TenantAdmin,
            IsActive = true
        });

        tenantDbContext.ServiceCategories.Add(new ServiceCategory
        {
            Name = "General Service",
            Description = "Default tenant category",
            DefaultDurationHrs = 1,
            DefaultRateAed = 100m
        });

        tenantDbContext.TenantSettings.Add(new TenantSettings
        {
            WorkOrderLabel = "Work Order",
            NotificationTemplatesJson = "{}",
            SlaRulesJson = "{}"
        });

        await tenantDbContext.SaveChangesAsync(cancellationToken);
    }
}
