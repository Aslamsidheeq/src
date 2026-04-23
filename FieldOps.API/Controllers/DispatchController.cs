using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class DispatchController(IMediator mediator) : ControllerBase
{
    [HttpGet("available-workers")]
    public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dispatch", "GetAvailableWorkers"), cancellationToken));

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("Dispatch", "AssignTeam", request), cancellationToken));
}
