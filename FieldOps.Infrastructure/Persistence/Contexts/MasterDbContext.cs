using FieldOps.Application.Common.Interfaces;
using FieldOps.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace FieldOps.Infrastructure.Persistence.Contexts;

public sealed class MasterDbContext(DbContextOptions<MasterDbContext> options)
    : DbContext(options), IMasterDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(b =>
        {
            b.ToTable("tenants");
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Subdomain).IsUnique();
            b.Property(x => x.Name).HasMaxLength(200);
            b.Property(x => x.Subdomain).HasMaxLength(100);
            b.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Subscription>(b =>
        {
            b.ToTable("subscriptions");
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Tenant).WithMany(x => x.Subscriptions).HasForeignKey(x => x.TenantId);
            b.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
