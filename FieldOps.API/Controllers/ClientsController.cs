using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Clients", "Get"), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("Clients", "Create", request), cancellationToken));
}
