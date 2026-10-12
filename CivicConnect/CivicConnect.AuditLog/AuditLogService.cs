namespace CivicConnect.AuditLog;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IRequestHistoryStore _store;

    public AuditLogService(IRequestHistoryStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<RequestHistoryEntry>> GetHistoryAsync(int requestId, CancellationToken cancellationToken = default)
    {
        var entries = await _store.GetByRequestAsync(requestId, cancellationToken);
        return entries
            .Where(e => e.RequestId == requestId)
            .OrderBy(e => e.OccurredAt)
            .ThenBy(e => e.Id)
            .ToList();
    }

    public async Task<IReadOnlyList<TimelineItem>> GetTimelineAsync(int requestId, CancellationToken cancellationToken = default)
    {
        var history = await GetHistoryAsync(requestId, cancellationToken);
        var items = new List<TimelineItem>(history.Count);
        for (var i = 0; i < history.Count; i++)
        {
            TimeSpan? inPrevious = i == 0 ? null : history[i].OccurredAt - history[i - 1].OccurredAt;
            items.Add(new TimelineItem(history[i], inPrevious));
        }
        return items;
    }
}
