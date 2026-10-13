using CivicConnect.AuditLog.Domain;
using CivicConnect.AuditLog.Persistence;
using CivicConnect.Common;

namespace CivicConnect.AuditLog.Events;

/// The only write path into request_history. A failure is NOT swallowed: if the audit row
/// cannot be written, the caller must know (NFR-005).
internal sealed class AuditLogEventHandler(IAuditLogRepository repository)
    : IDomainEventHandler<RequestStatusChangedEvent>
{
    public Task HandleAsync(RequestStatusChangedEvent e, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(e);
        var entry = new AuditEntry(e.RequestId, e.OldStatus, e.NewStatus, e.ChangedBy, e.OccurredAt);
        return repository.AppendAsync(entry, ct);
    }
}