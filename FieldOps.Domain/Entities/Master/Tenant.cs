using FieldOps.Domain.Enums;

namespace FieldOps.Domain.Entities.Master;

public sealed class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string Trn { get; set; } = string.Empty;
    public string Plan { get; set; } = "Standard";
    public string Industry { get; set; } = "General Field Service";
    public bool IsActive { get; set; } = true;
    public string DatabaseName { get; set; } = string.Empty;
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
