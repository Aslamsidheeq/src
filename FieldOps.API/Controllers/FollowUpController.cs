using FieldOps.Application.Features.Platform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class FollowUpController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderQuery("FollowUp", "GetHistory"), cancellationToken));

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] object request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new PlaceholderCommand("FollowUp", "Send", request), cancellationToken));
}
