namespace FieldOps.Application.Common.Models;

public sealed class Result<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public string? Code { get; init; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string error, string code) => new() { Success = false, Error = error, Code = code };
}

public sealed class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long Total { get; init; }
}

public sealed class TenantContext
{
    public Guid TenantId { get; init; }
    public string TenantName { get; init; } = string.Empty;
    public string Subdomain { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string WorkOrderNotFound = "WORK_ORDER_NOT_FOUND";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
}
