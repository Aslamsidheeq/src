using FieldOps.Application.Common.Models;
using FieldOps.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.API.Middleware;

public sealed class TenantMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public async Task Invoke(HttpContext context, MasterDbContext masterDbContext)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var subdomain = parts.Length >= 3 ? parts[0] : "local";

        var tenant = await masterDbContext.Tenants.FirstOrDefaultAsync(x => x.Subdomain == subdomain && x.IsActive);
        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Tenant not found or suspended.",
                code = ErrorCodes.TenantNotFound,
                timestamp = DateTime.UtcNow
            });
            return;
        }

        context.Items["TenantContext"] = new TenantContext
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            Subdomain = subdomain,
            DatabaseName = tenant.DatabaseName,
            IsActive = tenant.IsActive
        };

        await next(context);
    }
}
