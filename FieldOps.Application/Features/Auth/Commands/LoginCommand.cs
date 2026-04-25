using FieldOps.Application.Common.Interfaces;
using FieldOps.Application.Common.Models;
using FieldOps.Application.Features.Auth.DTOs;
using FieldOps.Domain.Entities.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Features.Auth.Commands;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

public sealed class LoginCommandHandler(
    IMasterDbContext masterDb,
    ITenantDbContextFactory tenantDbContextFactory,
    IJwtService jwtService,
    IPasswordService passwordService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenant = await masterDb.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.Subdomain == request.Request.Subdomain && t.IsActive,
            cancellationToken);

        if (tenant is null)
        {
            return Result<AuthResponse>.Fail("Tenant not found or suspended.", ErrorCodes.TenantNotFound);
        }

        await using var tenantDb = tenantDbContextFactory.Create(tenant.DatabaseName);

        var user = await tenantDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Email == request.Request.Email && u.IsActive,
                cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Fail("Invalid email or password.", ErrorCodes.InvalidCredentials);
        }

        if (!passwordService.Verify(request.Request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Fail("Invalid email or password.", ErrorCodes.InvalidCredentials);
        }

        var branch = await tenantDb.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == user.BranchId, cancellationToken);

        var accessToken = jwtService.GenerateToken(user, tenant, branch);

        await using var tenantDbWrite = tenantDbContextFactory.Create(tenant.DatabaseName);
        var refresh = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        tenantDbWrite.RefreshTokens.Add(refresh);
        await tenantDbWrite.SaveChangesAsync(cancellationToken);

        var redirectPath = user.Role switch
        {
            Domain.Enums.UserRole.TenantAdmin => "/dashboard/tenant-admin",
            Domain.Enums.UserRole.BranchManager => "/dashboard/branch-manager",
            Domain.Enums.UserRole.Supervisor => "/dashboard/supervisor",
            Domain.Enums.UserRole.Accountant => "/dashboard/accountant",
            Domain.Enums.UserRole.HRManager => "/dashboard/hr",
            _ => "/dashboard"
        };

        return Result<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = refresh.Token,
            RedirectPath = redirectPath,
            Role = user.Role.ToString(),
            UserId = user.Id,
            BranchId = user.Role == Domain.Enums.UserRole.TenantAdmin ? null : user.BranchId,
            Email = user.Email,
            TenantId = tenant.Id.ToString(),
            TenantDb = tenant.DatabaseName
        });
    }
}
