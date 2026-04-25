using System.Security.Claims;
using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FieldOps.Infrastructure.Identity;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? new ClaimsPrincipal();

    // Private helper — replaces FindFirstValue (requires Microsoft.AspNetCore.Identity)
    private string? Claim(string type) => User.FindFirst(type)?.Value;
    private string? ClaimAny(params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = Claim(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    public int UserId =>
        int.TryParse(ClaimAny(ClaimTypes.NameIdentifier, "sub"), out var id)
            ? id
            : 0;

    public string TenantId => Claim("tenant_id") ?? string.Empty;

    public string TenantDb => Claim("tenant_db") ?? string.Empty;

    public int? BranchId =>
        int.TryParse(Claim("branch_id"), out var branchId) && branchId > 0
            ? branchId
            : null;

    public UserRole Role =>
        Enum.TryParse<UserRole>(ClaimAny("role", ClaimTypes.Role), ignoreCase: true, out var role)
            ? role
            : UserRole.Supervisor;

    public string Email => ClaimAny("email", ClaimTypes.Email) ?? string.Empty;

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public bool IsTenantAdmin => Role == UserRole.TenantAdmin;
}