using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Infrastructure.Persistence.Contexts;

public sealed class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options), ITenantDbContext
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<FieldWorker> FieldWorkers => Set<FieldWorker>();
    public DbSet<WorkerDocument> WorkerDocuments => Set<WorkerDocument>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientSite> ClientSites => Set<ClientSite>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderAssignment> WorkOrderAssignments => Set<WorkOrderAssignment>();
    public DbSet<WorkOrderPhoto> WorkOrderPhotos => Set<WorkOrderPhoto>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<FieldWorker>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<WorkerDocument>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Client>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<ClientSite>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<ServiceCategory>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<WorkOrder>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<WorkOrderAssignment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<WorkOrderPhoto>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Attendance>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Invoice>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<InvoiceLineItem>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<FollowUp>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<TenantSettings>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Invoice>().Property(x => x.SubtotalAed).HasPrecision(10, 2);
        modelBuilder.Entity<Invoice>().Property(x => x.VatAed).HasPrecision(10, 2);
        modelBuilder.Entity<Invoice>().Property(x => x.TotalAed).HasPrecision(10, 2);
    }
}
