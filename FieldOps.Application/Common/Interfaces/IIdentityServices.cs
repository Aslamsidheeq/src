using FieldOps.Domain.Entities.Tenant;

namespace FieldOps.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, string tenantId, string tenantDb, Guid? branchId);
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
