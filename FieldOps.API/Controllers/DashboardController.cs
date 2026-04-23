using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("tenant-admin")]
    public async Task<IActionResult> TenantAdmin(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dashboard", "TenantAdmin"), cancellationToken));

    [HttpGet("branch-manager")]
    public async Task<IActionResult> BranchManager(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dashboard", "BranchManager"), cancellationToken));

    [HttpGet("supervisor")]
    public async Task<IActionResult> Supervisor(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dashboard", "Supervisor"), cancellationToken));

    [HttpGet("accountant")]
    public async Task<IActionResult> Accountant(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dashboard", "Accountant"), cancellationToken));

    [HttpGet("hr")]
    public async Task<IActionResult> Hr(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("Dashboard", "HR"), cancellationToken));
}
