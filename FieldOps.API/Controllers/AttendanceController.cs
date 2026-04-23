using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class AttendanceController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Attendance", "Get"), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Record([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("Attendance", "Record", request), cancellationToken));
}
