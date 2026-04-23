using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class WorkOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("WorkOrders", "Get"), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("WorkOrders", "Create", request), cancellationToken));

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("WorkOrders", $"UpdateStatus:{id}", request), cancellationToken));
}
