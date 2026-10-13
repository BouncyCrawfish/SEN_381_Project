namespace CivicConnect.Requests.Domain;

/// <summary>Maps to request_history. Append-only (NFR-005): there are no setters and no edit methods.</summary>
public sealed class RequestHistoryEntry
{
    private RequestHistoryEntry() { }   // EF Core

    public long Id { get; private set; }
    public long RequestId { get; private set; }
    public RequestStatus? OldStatus { get; private set; }   // null for the initial entry
    public RequestStatus NewStatus { get; private set; }
    public long ChangedBy { get; private set; }
    public DateTime Timestamp { get; private set; }

    internal static RequestHistoryEntry Create(RequestStatus? oldStatus, RequestStatus newStatus, long changedBy, DateTime timestampUtc) =>
        new() { OldStatus = oldStatus, NewStatus = newStatus, ChangedBy = changedBy, Timestamp = timestampUtc };
}

/// <summary>Maps to notes. Append-only: the application role has no UPDATE or DELETE on this table.</summary>
public sealed class RequestNote
{
    private RequestNote() { }   // EF Core

    public long Id { get; private set; }
    public long RequestId { get; private set; }
    public long AuthorId { get; private set; }
    public string Note { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }

    internal static RequestNote Create(long authorId, string note, DateTime timestampUtc) =>
        new() { AuthorId = authorId, Note = note, Timestamp = timestampUtc };
}

/// <summary>Maps to category (controlled list, FR-002).</summary>
public sealed class RequestCategory
{
    private RequestCategory() { }   // EF Core

    public RequestCategory(long id, string name, bool isActive = true)
    {
        Id = id; Name = name; IsActive = isActive;
    }

    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}
