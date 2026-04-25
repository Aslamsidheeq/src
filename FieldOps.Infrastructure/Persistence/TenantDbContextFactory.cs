using FieldOps.Application.Common.Interfaces;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace FieldOps.Infrastructure.Persistence;

public sealed class TenantDbContextFactory(IConfiguration configuration) : ITenantDbContextFactory
{
    public ITenantDbContext Create(string databaseName)
    {
        var masterConnStr = configuration.GetConnectionString("MasterDb")
            ?? throw new InvalidOperationException("ConnectionStrings:MasterDb is missing.");

        var tenantConnStr = new MySqlConnectionStringBuilder(masterConnStr)
        {
            Database = databaseName
        }.ConnectionString;

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseMySql(tenantConnStr, ServerVersion.AutoDetect(tenantConnStr))
            .Options;

        return new TenantDbContext(options);
    }
}
