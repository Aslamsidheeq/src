using System.Text;
using FieldOps.API.Extensions;
using FieldOps.API.Hubs;
using FieldOps.API.Middleware;
using FieldOps.Application.Common.Behaviors;
using FieldOps.Application.Common.Interfaces;
using FieldOps.Infrastructure.Identity;
using FieldOps.Infrastructure.Persistence;
using FieldOps.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FieldOps API", Version = "v1" });

    // Allows sending X-Tenant header from Swagger UI
    c.AddSecurityDefinition("X-Tenant", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-Tenant",
        Description = "Tenant subdomain (e.g. airalight)"
    });

    // JWT Bearer for authenticated endpoints
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT token here"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "X-Tenant"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MasterDb"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MasterDb"))));
builder.Services.AddSingleton<TenantConnectionStringFactory>();
builder.Services.AddDbContext<TenantDbContext>((sp, options) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var tenantConnectionString = httpContextAccessor.HttpContext
        ?.Items[TenantRequestContext.TenantConnectionStringKey] as string;

    // Fall back to MasterDb during startup, migrations, or provisioner usage
    var connStr = !string.IsNullOrWhiteSpace(tenantConnectionString)
        ? tenantConnectionString
        : builder.Configuration.GetConnectionString("MasterDb")!;

    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
});

builder.Services.AddScoped<IMasterDbContext>(sp => sp.GetRequiredService<MasterDbContext>());
builder.Services.AddScoped<ITenantDbContext>(sp => sp.GetRequiredService<TenantDbContext>());
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<TenantSchemaProvisioner>();
builder.Services.AddScoped<ITenantProvisioningService>(sp => sp.GetRequiredService<TenantSchemaProvisioner>());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(FieldOps.Application.Features.Auth.Commands.LoginCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddApplicationJwt(builder.Configuration);

builder.Services.AddCors(opt => opt.AddPolicy("DefaultCors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var masterDbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
    await masterDbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<BranchScopeMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<WorkOrderHub>("/hubs/work-orders");
app.Run();
