namespace FieldOps.Application.Common.Interfaces;

public interface ITenantDbContextFactory
{
    /// <summary>
    /// Creates a brand-new tenant context connected to the specified
    /// tenant database. Caller is responsible for disposing it.
    /// </summary>
    ITenantDbContext Create(string databaseName);
}
