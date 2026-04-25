using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Auth.DTOs;
using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;

public sealed class RefreshTokenCommandHandler(
    IMasterDbContext masterDb,
    ITenantDbContextFactory tenantDbContextFactory,
    IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Claim> claims;
        try
        {
            claims = jwtService.GetClaimsFromExpiredToken(request.AccessToken);
        }
        catch
        {
            return Result<AuthResponse>.Fail("Invalid token.", ErrorCodes.Unauthorized);
        }

        var tenantDb = claims.FirstOrDefault(c => c.Type == "tenant_db")?.Value;
        var userIdStr = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(tenantDb) || !int.TryParse(userIdStr, out var userId))
        {
            return Result<AuthResponse>.Fail("Invalid token.", ErrorCodes.Unauthorized);
        }

        await using var db = tenantDbContextFactory.Create(tenantDb);

        var existing = await db.RefreshTokens
            .FirstOrDefaultAsync(
                t => t.Token == request.RefreshToken
                     && t.UserId == userId
                     && t.RevokedAtUtc == null
                     && t.ExpiresAtUtc > DateTime.UtcNow,
                cancellationToken);

        if (existing is null)
        {
            return Result<AuthResponse>.Fail("Invalid or expired refresh token.", ErrorCodes.Unauthorized);
        }

        existing.RevokedAtUtc = DateTime.UtcNow;
        var rotated = new Domain.Entities.Tenant.RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        db.RefreshTokens.Add(rotated);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var tenant = await masterDb.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.DatabaseName == tenantDb, cancellationToken);

        if (user is null || tenant is null)
        {
            return Result<AuthResponse>.Fail("User or tenant not found.", ErrorCodes.TenantNotFound);
        }

        await db.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateToken(user, tenant, null);

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = rotated.Token,
            Role = user.Role.ToString(),
            UserId = user.Id,
            BranchId = user.BranchId,
            Email = user.Email,
            TenantId = tenant.Id.ToString(),
            TenantDb = tenant.DatabaseName
        });
    }
}
