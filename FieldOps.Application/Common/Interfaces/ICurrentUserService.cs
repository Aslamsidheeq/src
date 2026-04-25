using FieldOps.Domain.Enums;

namespace FieldOps.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string TenantId { get; }
    string TenantDb { get; }
    int? BranchId { get; } // null for TenantAdmin
    UserRole Role { get; }
    string Email { get; }
    bool IsTenantAdmin { get; }
}
