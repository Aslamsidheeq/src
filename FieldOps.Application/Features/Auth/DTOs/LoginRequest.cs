namespace FieldOps.Application.Features.Auth.DTOs;

public sealed class LoginRequest
{
    public string Subdomain { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
