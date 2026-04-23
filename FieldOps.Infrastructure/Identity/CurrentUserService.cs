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

    public Guid UserId =>
        Guid.TryParse(Claim(ClaimTypes.NameIdentifier) ?? Claim("sub"), out var id)
            ? id
            : Guid.Empty;

    public string TenantId => Claim("tenant_id") ?? string.Empty;

    public string TenantDb => Claim("tenant_db") ?? string.Empty;

    public Guid? BranchId =>
        Guid.TryParse(Claim("branch_id"), out var branchId)
            ? branchId
            : null;

    public UserRole Role =>
        Enum.TryParse<UserRole>(Claim("role"), ignoreCase: true, out var role)
            ? role
            : UserRole.Supervisor;

    public string Email => Claim("email") ?? string.Empty;

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public bool IsTenantAdmin => Role == UserRole.TenantAdmin;
}