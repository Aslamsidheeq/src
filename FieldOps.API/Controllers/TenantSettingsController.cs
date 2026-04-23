using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TenantSettingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("TenantSettings", "Get"), cancellationToken));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("TenantSettings", "Update", request), cancellationToken));
}
