using FieldOps.Domain.Enums;

namespace FieldOps.Application.Features.Branches.DTOs;

public sealed class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Emirate { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Trn { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string InvoicePrefix { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsActive { get; set; }
}

