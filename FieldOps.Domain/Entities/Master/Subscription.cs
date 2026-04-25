namespace FieldOps.Domain.Entities.Master;

public sealed class Subscription : BaseEntity
{
    public int TenantId { get; set; }
    public string Plan { get; set; } = "Standard";
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant? Tenant { get; set; }
}
