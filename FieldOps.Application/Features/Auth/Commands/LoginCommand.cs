using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Auth.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Subdomain).NotEmpty();
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Password).NotEmpty().MinimumLength(8);
    }
}

public sealed class LoginCommandHandler(
    IMasterDbContext masterDbContext,
    ITenantDbContext tenantDbContext,
    IPasswordService passwordService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenant = await masterDbContext.Tenants.FirstOrDefaultAsync(
            x => x.Subdomain == request.Request.Subdomain && x.IsActive,
            cancellationToken);

        if (tenant is null)
        {
            return Result<AuthResponse>.Fail("Tenant not found.", ErrorCodes.TenantNotFound);
        }

        var user = await tenantDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Request.Email && x.IsActive, cancellationToken);
        if (user is null || !passwordService.Verify(request.Request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Fail("Invalid credentials.", ErrorCodes.InvalidCredentials);
        }

        var accessToken = jwtService.GenerateAccessToken(user, tenant.Id.ToString(), tenant.DatabaseName, user.Role == Domain.Enums.UserRole.TenantAdmin ? null : user.BranchId);
        var refresh = new Domain.Entities.Tenant.RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenService.GenerateSecureToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        tenantDbContext.RefreshTokens.Add(refresh);
        await tenantDbContext.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = refresh.Token,
            Role = user.Role.ToString(),
            UserId = user.Id,
            BranchId = user.Role == Domain.Enums.UserRole.TenantAdmin ? null : user.BranchId,
            Email = user.Email,
            TenantId = tenant.Id.ToString(),
            TenantDb = tenant.DatabaseName
        });
    }
}
