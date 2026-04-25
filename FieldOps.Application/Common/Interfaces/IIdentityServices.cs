using FieldOps.Domain.Entities.Tenant;
using FieldOps.Domain.Entities.Master;
using System.Security.Claims;

namespace FieldOps.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, Tenant tenant, Branch? branch);
    IEnumerable<Claim> GetClaimsFromExpiredToken(string accessToken);
}

public interface IRefreshTokenService
{
    string GenerateSecureToken();
    Task<RefreshToken> RotateAsync(RefreshToken currentToken, CancellationToken cancellationToken);
}

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
