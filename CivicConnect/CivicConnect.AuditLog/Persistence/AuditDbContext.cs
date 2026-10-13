using CivicConnect.AuditLog.Domain;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.AuditLog.Persistence;

/// Maps ONLY request_history. The schema is owned by db/03_schema.sql, so do NOT generate EF migrations.
internal sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditEntry> Entries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEntry>(b =>
        {
            b.ToTable("request_history");
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(e => e.RequestId).HasColumnName("request_id").IsRequired();
            b.Property(e => e.OldStatus).HasColumnName("old_status").HasMaxLength(20);
            b.Property(e => e.NewStatus).HasColumnName("new_status").HasMaxLength(20).IsRequired();
            b.Property(e => e.ChangedBy).HasColumnName("changed_by").IsRequired();
            b.Property(e => e.Timestamp).HasColumnName("timestamp").IsRequired();
        });
    }
}