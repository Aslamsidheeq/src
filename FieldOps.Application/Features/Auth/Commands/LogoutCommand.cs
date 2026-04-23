using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

public sealed class LogoutCommandHandler(ITenantDbContext tenantDbContext) : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await tenantDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);
        if (token is null)
        {
            return Result<bool>.Ok(true);
        }

        token.RevokedAtUtc = DateTime.UtcNow;
        token.UpdatedAtUtc = DateTime.UtcNow;
        await tenantDbContext.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}
