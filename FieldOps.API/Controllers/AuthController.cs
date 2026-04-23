using FieldOps.Application.Features.Auth.Commands;
using FieldOps.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(request), cancellationToken);
        return Ok(new { success = result.Success, data = result.Data, error = result.Error, code = result.Code, timestamp = DateTime.UtcNow });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"] ?? string.Empty;
        var result = await mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);
        return Ok(new { success = result.Success, data = result.Data, error = result.Error, code = result.Code, timestamp = DateTime.UtcNow });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"] ?? string.Empty;
        var result = await mediator.Send(new LogoutCommand(refreshToken), cancellationToken);
        return Ok(new { success = result.Success, data = result.Data, error = result.Error, code = result.Code, timestamp = DateTime.UtcNow });
    }
}
