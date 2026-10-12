using CivicConnect.SharedKernel;

namespace CivicConnect.AuditLog;


public sealed class AuditLogRecorder : IDomainEventHandler<RequestStatusChangedEvent>
{
    private readonly IRequestHistoryStore _store;

    public AuditLogRecorder(IRequestHistoryStore store)
    {
        _store = store;
    }

    public Task HandleAsync(RequestStatusChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent.RequestId <= 0)
            throw new ArgumentException("RequestId must be positive.", nameof(domainEvent));
        if (domainEvent.ChangedByUserId <= 0)
            throw new ArgumentException("ChangedByUserId is required for attribution.", nameof(domainEvent));

        var entry = new RequestHistoryEntry(
            domainEvent.RequestId,
            domainEvent.OldStatus,
            domainEvent.NewStatus,
            domainEvent.ChangedByUserId,
            domainEvent.OccurredAt);

        return _store.AppendAsync(entry, cancellationToken);
    }
}
