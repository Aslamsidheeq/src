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
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MasterDb"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MasterDb"))));
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MasterDb"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MasterDb"))));

builder.Services.AddScoped<IMasterDbContext>(sp => sp.GetRequiredService<MasterDbContext>());
builder.Services.AddScoped<ITenantDbContext>(sp => sp.GetRequiredService<TenantDbContext>());
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<TenantSchemaProvisioner>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(FieldOps.Application.Features.Auth.Commands.LoginCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddApplicationJwt(builder.Configuration);

builder.Services.AddCors(opt => opt.AddPolicy("DefaultCors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
// var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var masterDbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
//     var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

//     await masterDbContext.Database.EnsureCreatedAsync();
//     await tenantDbContext.Database.EnsureCreatedAsync();
// }

// app.UseMiddleware<ExceptionMiddleware>();
// app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<BranchScopeMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<WorkOrderHub>("/hubs/work-orders");
app.Run();
