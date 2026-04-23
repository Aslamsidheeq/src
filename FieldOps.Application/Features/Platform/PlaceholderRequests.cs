using FieldOps.Application.Common.Models;
using MediatR;

namespace FieldOps.Application.Features.Platform;

public sealed record PlaceholderQuery(string Feature, string Action) : IRequest<Result<object>>;
public sealed record PlaceholderCommand(string Feature, string Action, object Payload) : IRequest<Result<object>>;

public sealed class PlaceholderQueryHandler : IRequestHandler<PlaceholderQuery, Result<object>>
{
    public Task<Result<object>> Handle(PlaceholderQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result<object>.Ok(new
        {
            request.Feature,
            request.Action,
            generatedAtUtc = DateTime.UtcNow
        } as object));
}

public sealed class PlaceholderCommandHandler : IRequestHandler<PlaceholderCommand, Result<object>>
{
    public Task<Result<object>> Handle(PlaceholderCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result<object>.Ok(new
        {
            request.Feature,
            request.Action,
            request.Payload,
            generatedAtUtc = DateTime.UtcNow
        } as object));
}
