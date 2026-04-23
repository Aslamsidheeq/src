using MySqlConnector;

namespace FieldOps.Infrastructure.Persistence;

public sealed class TenantConnectionStringFactory
{
    public string Build(string baseConnectionString, string databaseName)
    {
        var builder = new MySqlConnectionStringBuilder(baseConnectionString)
        {
            Database = databaseName
        };
        return builder.ConnectionString;
    }
}
