using FieldOps.Domain.Entities.Master;
using FieldOps.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Application.Common.Interfaces;

public interface IMasterDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Subscription> Subscriptions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ITenantDbContext : IAsyncDisposable
{
    DbSet<Branch> Branches { get; }
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<FieldWorker> FieldWorkers { get; }
    DbSet<WorkerDocument> WorkerDocuments { get; }
    DbSet<Client> Clients { get; }
    DbSet<ClientSite> ClientSites { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<WorkOrderAssignment> WorkOrderAssignments { get; }
    DbSet<WorkOrderPhoto> WorkOrderPhotos { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceLineItem> InvoiceLineItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<FollowUp> FollowUps { get; }
    DbSet<TenantSettings> TenantSettings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
