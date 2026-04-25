using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record LogoutCommand : IRequest<Result<bool>>;

public sealed class LogoutCommandHandler(
    ITenantDbContextFactory tenantDbContextFactory,
    ICurrentUserService currentUser) : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tenantDb = currentUser.TenantDb;
        if (string.IsNullOrWhiteSpace(tenantDb) || currentUser.UserId <= 0)
        {
            return Result<bool>.Ok(true);
        }

        await using var db = tenantDbContextFactory.Create(tenantDb);
        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == currentUser.UserId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}
