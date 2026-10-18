using CivicConnect.AuditLog.Contracts;
using CivicConnect.AuditLog.Domain;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.AuditLog.Persistence;

internal interface IAuditLogRepository
{
    Task AppendAsync(AuditEntry entry, CancellationToken ct);
    Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQueryDto query, int maxRows, CancellationToken ct);
}

internal sealed class EfAuditLogRepository(IDbContextFactory<AuditDbContext> factory) : IAuditLogRepository
{
    public async Task AppendAsync(AuditEntry entry, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Entries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQueryDto q, int maxRows, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        IQueryable<AuditEntry> entries = db.Entries.AsNoTracking();

        if (q.RequestId is { } r) entries = entries.Where(e => e.RequestId == r);
        if (q.ChangedBy is { } u) entries = entries.Where(e => e.ChangedBy == u);
        if (!string.IsNullOrWhiteSpace(q.NewStatus)) entries = entries.Where(e => e.NewStatus == q.NewStatus);
        if (q.From is { } from) entries = entries.Where(e => e.Timestamp >= from.ToUniversalTime());
        if (q.To is { } to) entries = entries.Where(e => e.Timestamp <= to.ToUniversalTime());

        return await entries.OrderBy(e => e.Timestamp).ThenBy(e => e.Id).Take(maxRows).ToListAsync(ct);
    }
}