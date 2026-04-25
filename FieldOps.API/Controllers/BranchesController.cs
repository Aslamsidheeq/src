using FieldOps.Application.Features.Branches.Commands;
using FieldOps.Application.Features.Branches.DTOs;
using FieldOps.Application.Features.Branches.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class BranchesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new GetBranchesQuery(page, pageSize, search), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetBranchByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new CreateBranchCommand(request), cancellationToken));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "TenantAdmin,BranchManager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new UpdateBranchCommand(id, request), cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new DeleteBranchCommand(id), cancellationToken));
}
