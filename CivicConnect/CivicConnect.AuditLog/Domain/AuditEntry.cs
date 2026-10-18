namespace CivicConnect.AuditLog.Domain;

/// One row of request_history. No setters are public: an entry never changes after it is created.
internal sealed class AuditEntry
{
    private AuditEntry() { } // EF Core

    public AuditEntry(long requestId, string? oldStatus, string newStatus, long changedBy, DateTimeOffset timestamp)
    {
        RequestId = requestId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedBy = changedBy;
        Timestamp = timestamp.ToUniversalTime();
    }

    public long Id { get; private set; }
    public long RequestId { get; private set; }
    public string? OldStatus { get; private set; }
    public string NewStatus { get; private set; } = string.Empty;
    public long ChangedBy { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
}