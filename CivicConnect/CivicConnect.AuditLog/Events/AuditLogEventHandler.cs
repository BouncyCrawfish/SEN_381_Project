using CivicConnect.AuditLog.Domain;
using CivicConnect.AuditLog.Persistence;
using CivicConnect.Common;

namespace CivicConnect.AuditLog.Events;

internal sealed class AuditLogEventHandler : IDomainEventHandler<RequestStatusChangedEvent>
{
    private readonly IAuditLogRepository _repo;

    public AuditLogEventHandler(IAuditLogRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public Task HandleAsync(RequestStatusChangedEvent evt, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(evt);

        var entry = new AuditEntry(
            evt.RequestId,
            evt.OldStatus,
            evt.NewStatus,
            evt.ChangedBy,
            evt.OccurredAt); // the constructor already converts to UTC

        return _repo.AppendAsync(entry, ct);
    }
}