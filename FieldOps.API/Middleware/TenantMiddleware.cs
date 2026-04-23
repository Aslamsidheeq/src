using FieldOps.Application.Common.Models;
using FieldOps.Infrastructure.Persistence;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.API.Middleware;

public sealed class TenantMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    TenantConnectionStringFactory tenantConnectionStringFactory)
{
    public async Task Invoke(HttpContext context, MasterDbContext masterDbContext)
{
    var path   = context.Request.Path.Value ?? "";
    var method = context.Request.Method;

    // Skip tenant resolution for infrastructure paths
    if (path.StartsWith("/swagger",      StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/health",        StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/dev",           StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/api/tenants",   StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/favicon",       StringComparison.OrdinalIgnoreCase))
    {
        await next(context);
        return;
    }

    // Resolve subdomain
    var host = context.Request.Host.Host;

    // Dev: read from X-Tenant header when on localhost
    string subdomain;
    if (host == "localhost" || host == "127.0.0.1")
    {
        subdomain = context.Request.Headers["X-Tenant"].FirstOrDefault() ?? string.Empty;
    }
    else
    {
        var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
        subdomain = parts.Length >= 3 ? parts[0] : string.Empty;
    }

    // If no tenant context available, skip silently
    // (allows anonymous endpoints like /api/auth/login to self-identify via X-Tenant)
    if (string.IsNullOrWhiteSpace(subdomain))
    {
        await next(context);
        return;
    }

    var tenant = await masterDbContext.Tenants
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Subdomain == subdomain && x.IsActive);

    if (tenant is null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new
        {
            success  = false,
            error    = "Tenant not found or suspended.",
            code     = ErrorCodes.TenantNotFound,
            timestamp = DateTime.UtcNow
        });
        return;
    }

    var masterConnectionString = configuration.GetConnectionString("MasterDb")
        ?? throw new InvalidOperationException("ConnectionStrings:MasterDb is missing.");
    var tenantConnectionString = tenantConnectionStringFactory.Build(
        masterConnectionString, tenant.DatabaseName);

    context.Items[TenantRequestContext.TenantConnectionStringKey] = tenantConnectionString;
    context.Items[TenantRequestContext.TenantContextKey] = new TenantContext
    {
        TenantId   = tenant.Id,
        TenantName = tenant.Name,
        Subdomain  = subdomain,
        DatabaseName = tenant.DatabaseName,
        IsActive   = tenant.IsActive
    };

    await next(context);
}
}
