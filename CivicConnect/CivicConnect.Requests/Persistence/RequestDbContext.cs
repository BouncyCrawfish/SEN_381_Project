using CivicConnect.Requests.Domain;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.Requests.Persistence;

/// <summary>
/// Maps request, request_history, notes and category. The schema is owned by db/*.sql, so do NOT
/// generate EF migrations for this context (same rule as AuthDbContext).
/// </summary>
internal sealed class RequestsDbContext(DbContextOptions<RequestsDbContext> options) : DbContext(options)
{
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestHistoryEntry> History => Set<RequestHistoryEntry>();
    public DbSet<RequestNote> Notes => Set<RequestNote>();
    public DbSet<RequestCategory> Categories => Set<RequestCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestCategory>(b =>
        {
            b.ToTable("category");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            b.Property(c => c.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<Request>(b =>
        {
            b.ToTable("request");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(r => r.RequesterId).HasColumnName("requester_id");
            b.Property(r => r.AssignedStaffId).HasColumnName("assigned_staff_id");
            b.Property(r => r.CategoryId).HasColumnName("category_id");
            b.Property(r => r.Description).HasColumnName("description").IsRequired();
            b.Property(r => r.Location).HasColumnName("location").HasMaxLength(RequestRules.MaxLocationLength).IsRequired();
            b.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasConversion<string>().IsRequired();
            b.Property(r => r.CreatedAt).HasColumnName("created_at");

            // Optimistic concurrency on PostgreSQL's xmin system column (ADR-002).
            b.Property(r => r.Version).HasColumnName("xmin").HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();

            b.Ignore(r => r.State);
            b.HasMany(r => r.History).WithOne().HasForeignKey(h => h.RequestId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(r => r.Notes).WithOne().HasForeignKey(n => n.RequestId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(r => r.History).HasField("_history").UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation(r => r.Notes).HasField("_notes").UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<RequestHistoryEntry>(b =>
        {
            b.ToTable("request_history");
            b.HasKey(h => h.Id);
            b.Property(h => h.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(h => h.RequestId).HasColumnName("request_id");
            b.Property(h => h.OldStatus).HasColumnName("old_status").HasMaxLength(20).HasConversion<string>();
            b.Property(h => h.NewStatus).HasColumnName("new_status").HasMaxLength(20).HasConversion<string>().IsRequired();
            b.Property(h => h.ChangedBy).HasColumnName("changed_by");
            b.Property(h => h.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<RequestNote>(b =>
        {
            b.ToTable("notes");
            b.HasKey(n => n.Id);
            b.Property(n => n.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(n => n.RequestId).HasColumnName("request_id");
            b.Property(n => n.AuthorId).HasColumnName("author_id");
            b.Property(n => n.Note).HasColumnName("note").IsRequired();
            b.Property(n => n.Timestamp).HasColumnName("timestamp");
        });
    }
}
