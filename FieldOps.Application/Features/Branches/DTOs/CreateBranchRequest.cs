using FieldOps.Domain.Enums;

namespace FieldOps.Application.Features.Branches.DTOs;

public sealed class CreateBranchRequest
{
    public string Name { get; set; } = string.Empty;
    public Emirates Emirate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Trn { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string InvoicePrefix { get; set; } = "INV";
    public int? ManagerId { get; set; }
}

