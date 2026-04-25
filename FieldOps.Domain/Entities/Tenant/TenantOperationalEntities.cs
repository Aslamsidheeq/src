using FieldOps.Domain.Enums;

namespace FieldOps.Domain.Entities.Tenant;

public sealed class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Emirates Emirate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Trn { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string InvoicePrefix { get; set; } = "INV";
    public int? ManagerId { get; set; }
}

public sealed class User : BaseEntity
{
    public int BranchId { get; set; }
    public UserRole Role { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}

public sealed class FieldWorker : BaseEntity
{
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime? VisaExpiryUtc { get; set; }
    public DateTime? EidExpiryUtc { get; set; }
    public WorkerStatus Status { get; set; } = WorkerStatus.Active;
    public ICollection<WorkerSkill> Skills { get; set; } = new List<WorkerSkill>();
}

public sealed class WorkerDocument : BaseEntity
{
    public int WorkerId { get; set; }
    public DocumentType Type { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public DateTime? ExpiryDateUtc { get; set; }
}

public sealed class WorkerSkill : BaseEntity
{
    public int WorkerId { get; set; }
    public string SkillName { get; set; } = string.Empty;
}

public sealed class Client : BaseEntity
{
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ClientType Type { get; set; }
    public string? Trn { get; set; }
}

public sealed class ClientSite : BaseEntity
{
    public int ClientId { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public Emirates Emirate { get; set; }
    public string? Notes { get; set; }
}

public sealed class ServiceCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultDurationHrs { get; set; }
    public decimal DefaultRateAed { get; set; }
}

public sealed class WorkOrder : BaseEntity
{
    public int BranchId { get; set; }
    public int ClientId { get; set; }
    public int SiteId { get; set; }
    public int CategoryId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
    public string CustomFieldsJson { get; set; } = "{}";
}

public sealed class WorkOrderAssignment : BaseEntity
{
    public int WorkOrderId { get; set; }
    public int WorkerId { get; set; }
    public DateTime? CheckinTimeUtc { get; set; }
    public DateTime? CheckoutTimeUtc { get; set; }
    public string? Notes { get; set; }
}

public sealed class WorkOrderPhoto : BaseEntity
{
    public int WorkOrderId { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime TakenAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Attendance : BaseEntity
{
    public int WorkerId { get; set; }
    public int BranchId { get; set; }
    public DateTime DateUtc { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public sealed class Invoice : BaseEntity
{
    public int BranchId { get; set; }
    public int WorkOrderId { get; set; }
    public int ClientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal SubtotalAed { get; set; }
    public decimal VatAed { get; set; }
    public decimal TotalAed { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime IssueDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueDateUtc { get; set; }
    public string CompanyTrn { get; set; } = string.Empty;
    public int? OriginalInvoiceId { get; set; }
}

public sealed class InvoiceLineItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPriceAed { get; set; }
    public decimal VatAed { get; set; }
    public decimal TotalAed { get; set; }
}

public sealed class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public decimal AmountAed { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class FollowUp : BaseEntity
{
    public int WorkOrderId { get; set; }
    public int ClientId { get; set; }
    public FollowUpType Type { get; set; }
    public FollowUpChannel Channel { get; set; }
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    public string? Response { get; set; }
}

public sealed class TenantSettings : BaseEntity
{
    public string WorkOrderLabel { get; set; } = "Work Order";
    public string NotificationTemplatesJson { get; set; } = "{}";
    public string SlaRulesJson { get; set; } = "{}";
}
