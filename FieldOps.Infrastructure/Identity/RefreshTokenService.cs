using System.Security.Cryptography;
using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Entities.Tenant;

namespace FieldOps.Infrastructure.Identity;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string GenerateSecureToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public Task<RefreshToken> RotateAsync(RefreshToken currentToken, CancellationToken cancellationToken)
    {
        currentToken.RevokedAtUtc = DateTime.UtcNow;
        currentToken.UpdatedAtUtc = DateTime.UtcNow;
        var replacement = new RefreshToken
        {
            UserId = currentToken.UserId,
            Token = GenerateSecureToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        return Task.FromResult(replacement);
    }
}
