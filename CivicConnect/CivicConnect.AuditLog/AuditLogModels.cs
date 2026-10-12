using CivicConnect.SharedKernel;

namespace CivicConnect.AuditLog;

public sealed record RequestHistoryEntry(
    int RequestId,
    RequestStatus? OldStatus,
    RequestStatus NewStatus,
    int ChangedByUserId,
    DateTimeOffset OccurredAt)
{
    
    public long Id { get; init; }
}
public sealed record TimelineItem(RequestHistoryEntry Entry, TimeSpan? TimeInPreviousStatus);


public interface IRequestHistoryStore
{
    Task AppendAsync(RequestHistoryEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RequestHistoryEntry>> GetByRequestAsync(int requestId, CancellationToken cancellationToken = default);
}

public interface IAuditLogService
{
    Task<IReadOnlyList<RequestHistoryEntry>> GetHistoryAsync(int requestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimelineItem>> GetTimelineAsync(int requestId, CancellationToken cancellationToken = default);
}
