namespace FieldOps.API.Middleware;

public sealed class BranchScopeMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var branchId = context.User.FindFirst("branch_id")?.Value;
            context.Items["BranchScope"] = branchId;
        }
        await next(context);
    }
}
