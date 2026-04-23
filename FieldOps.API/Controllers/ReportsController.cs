using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ReportsController(IMediator mediator) : ControllerBase
{
    [HttpGet("operational")]
    public async Task<IActionResult> Operational(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Reports", "Operational"), cancellationToken));

    [HttpGet("financial")]
    public async Task<IActionResult> Financial(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Reports", "Financial"), cancellationToken));
}
