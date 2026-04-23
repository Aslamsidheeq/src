using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Entities.Master;
using FieldOps.Domain.Entities.Tenant;
using FieldOps.Domain.Enums;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FieldOps.Infrastructure.Persistence;

public sealed class TenantSchemaProvisioner(
    MasterDbContext masterDbContext,
    IConfiguration configuration,
    TenantConnectionStringFactory tenantConnectionStringFactory) : ITenantProvisioningService
{
    public async Task ProvisionAsync(Tenant tenant, string adminEmail, string adminPasswordHash, CancellationToken cancellationToken)
    {
        var tenantDatabaseName = NormalizeDatabaseName(tenant.DatabaseName);
        tenant.DatabaseName = tenantDatabaseName;

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

        var createDatabaseSql = $"CREATE DATABASE IF NOT EXISTS `{tenantDatabaseName}`;";
        await masterDbContext.Database.ExecuteSqlRawAsync(createDatabaseSql, cancellationToken);

        var masterConnectionString = configuration.GetConnectionString("MasterDb")
            ?? throw new InvalidOperationException("ConnectionStrings:MasterDb is missing.");
        var tenantConnectionString = tenantConnectionStringFactory.Build(masterConnectionString, tenantDatabaseName);
        var tenantOptions = new DbContextOptionsBuilder<TenantDbContext>()
            .UseMySql(tenantConnectionString, ServerVersion.AutoDetect(tenantConnectionString))
            .Options;

        await using var tenantDbContext = new TenantDbContext(tenantOptions);
        await tenantDbContext.Database.MigrateAsync(cancellationToken);

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

    private static string NormalizeDatabaseName(string databaseName)
    {
        var normalized = databaseName
            .Trim()
            .ToLowerInvariant()
            .Replace("-", "_");

        if (string.IsNullOrWhiteSpace(normalized) || normalized.Any(ch => !(char.IsLetterOrDigit(ch) || ch == '_')))
        {
            throw new InvalidOperationException("Tenant database name contains unsupported characters.");
        }

        return normalized;
    }
}
