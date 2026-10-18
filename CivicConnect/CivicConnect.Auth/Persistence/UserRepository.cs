using CivicConnect.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CivicConnect.Auth;

internal interface IUserRepository
{
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct);
    Task<User?> FindByIdAsync(long id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct);
    Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken ct);
    /// <summary>False when the unique email index rejected the insert (lost a race).</summary>
    Task<bool> TryAddAsync(User user, CancellationToken ct);
    Task UpdatePasswordHashAsync(long userId, string newHash, CancellationToken ct);
}

/// <summary>
/// One short-lived DbContext per call (factory), which suits WinForms: no long-lived context
/// holding stale tracked entities. All queries are parameterised by EF Core.
/// Only columns granted to civicconnect_app are ever written: insert, and UPDATE of password_hash.
/// The role column is never updated by the app.
/// </summary>
internal sealed class EfUserRepository(IDbContextFactory<AuthDbContext> factory) : IUserRepository
{
    public async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, ct);
    }

    public async Task<User?> FindByIdAsync(long id, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Users.AsNoTracking().AnyAsync(u => u.Email.ToLower() == normalizedEmail, ct);
    }

    public async Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Users.AsNoTracking().Where(u => u.Role == role).OrderBy(u => u.Name).ToListAsync(ct);
    }

    public async Task<bool> TryAddAsync(User user, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return false;
        }
    }

    public async Task UpdatePasswordHashAsync(long userId, string newHash, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await db.Users.Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.PasswordHash, newHash), ct);
    }
}
