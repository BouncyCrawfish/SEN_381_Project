using CivicConnect.Common;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.Auth;

/// <summary>
/// Maps ONLY the users table. The schema is owned by db/03_schema.sql, so do NOT generate EF
/// migrations for this context; it would try to create/alter tables the SQL scripts already own.
/// </summary>
internal sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            b.Property(u => u.Name).HasColumnName("name").HasMaxLength(AuthRules.MaxNameLength).IsRequired();
            b.Property(u => u.Email).HasColumnName("email").HasMaxLength(AuthRules.MaxEmailLength).IsRequired();
            b.Property(u => u.Role).HasColumnName("role").HasMaxLength(20).HasConversion<string>().IsRequired();
            b.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        });
    }
}
