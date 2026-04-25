namespace FieldOps.Application.Features.Auth.DTOs;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public string RedirectPath { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? BranchId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string TenantDb { get; set; } = string.Empty;
}
