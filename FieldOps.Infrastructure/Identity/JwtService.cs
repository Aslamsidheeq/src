using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Entities.Master;
using FieldOps.Domain.Entities.Tenant;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FieldOps.Infrastructure.Identity;

public sealed class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateToken(User user, Tenant tenant, Branch? branch)
    {
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing");
        var issuer = configuration["Jwt:Issuer"] ?? "fieldops";
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var parsed) ? parsed : 15;

        var branchClaimValue = user.Role == Domain.Enums.UserRole.TenantAdmin
            ? string.Empty
            : (branch?.Id ?? user.BranchId).ToString();
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("role", user.Role.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("tenant_id", tenant.Id.ToString()),
            new Claim("tenant_db", tenant.DatabaseName),
            new Claim("branch_id", branchClaimValue)
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public IEnumerable<Claim> GetClaimsFromExpiredToken(string accessToken)
    {
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing");
        var issuer = configuration["Jwt:Issuer"] ?? "fieldops";

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal.Claims;
    }
}
