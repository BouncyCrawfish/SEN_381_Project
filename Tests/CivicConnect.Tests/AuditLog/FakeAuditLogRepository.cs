using CivicConnect.AuditLog.Contracts;
using CivicConnect.AuditLog.Domain;
using CivicConnect.AuditLog.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Tests;

internal sealed class FakeAuditLogRepository : IAuditLogRepository
{
    public List<AuditEntry> Entries { get; } = new();

    public Task AppendAsync(AuditEntry entry, CancellationToken ct) { Entries.Add(entry); return Task.CompletedTask; }

    public Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQueryDto q, int maxRows, CancellationToken ct)
    {
        var rows = Entries.AsEnumerable();
        if (q.RequestId is { } r) rows = rows.Where(e => e.RequestId == r);
        if (q.ChangedBy is { } u) rows = rows.Where(e => e.ChangedBy == u);
        if (!string.IsNullOrWhiteSpace(q.NewStatus)) rows = rows.Where(e => e.NewStatus == q.NewStatus);
        return Task.FromResult<IReadOnlyList<AuditEntry>>(rows.Take(maxRows).ToList());
    }
}