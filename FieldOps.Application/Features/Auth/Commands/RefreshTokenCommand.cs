using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;

public sealed class RefreshTokenCommandHandler(
    ITenantDbContext tenantDbContext,
    ICurrentUserService currentUserService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await tenantDbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken && x.RevokedAtUtc == null, cancellationToken);
        if (existing is null || existing.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Result<AuthResponse>.Fail("Invalid refresh token.", ErrorCodes.Unauthorized);
        }

        var user = await tenantDbContext.Users.FirstAsync(x => x.Id == existing.UserId, cancellationToken);
        var rotated = await refreshTokenService.RotateAsync(existing, cancellationToken);
        tenantDbContext.RefreshTokens.Add(rotated);
        await tenantDbContext.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(user, currentUserService.TenantId, currentUserService.TenantDb, currentUserService.BranchId);
        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = rotated.Token,
            Role = user.Role.ToString(),
            UserId = user.Id,
            BranchId = currentUserService.BranchId,
            Email = user.Email,
            TenantId = currentUserService.TenantId,
            TenantDb = currentUserService.TenantDb
        });
    }
}
