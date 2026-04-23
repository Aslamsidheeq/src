using System.Net;
using System.Text.Json;
using FluentValidation;

namespace FieldOps.API.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await WriteErrorAsync(context, "Validation failed.", "VALIDATION_FAILED", ex.Errors.Select(x => x.ErrorMessage));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteErrorAsync(context, "Unexpected error occurred.", "UNEXPECTED_ERROR");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, string error, string code, object? details = null)
    {
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new
        {
            success = false,
            error,
            code,
            details,
            timestamp = DateTime.UtcNow
        });
        return context.Response.WriteAsync(payload);
    }
}
