using FieldOps.Domain.Enums;

namespace FieldOps.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string TenantId { get; }
    string TenantDb { get; }
    Guid? BranchId { get; } // null for TenantAdmin
    UserRole Role { get; }
    string Email { get; }
}
