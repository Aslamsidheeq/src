using Microsoft.AspNetCore.SignalR;

namespace FieldOps.API.Hubs;

public sealed class WorkOrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        var branchId = Context.User?.FindFirst("branch_id")?.Value;

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, tenantId);
            if (!string.IsNullOrWhiteSpace(branchId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"{tenantId}:{branchId}");
            }
        }

        await base.OnConnectedAsync();
    }
}
