using CivicConnect.Common;

namespace CivicConnect.Requests.Domain;

/// <summary>
/// Aggregate root for a service request. All changes go through methods that enforce the
/// State Pattern rules, so a request can never be put into an illegal status from outside.
/// Every successful transition appends a history entry (NFR-005) and, when given, a note (FR-012);
/// the repository saves the request and those new rows in ONE transaction (ADR-002).
/// </summary>
public class Request
{
    private readonly List<RequestHistoryEntry> _history = new();
    private readonly List<RequestNote> _notes = new();

    private Request() { }   // EF Core

    public long Id { get; private set; }
    public long RequesterId { get; private set; }
    public long? AssignedStaffId { get; private set; }
    public long CategoryId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public RequestStatus Status { get; private set; } = RequestStatus.Submitted;
    public DateTime CreatedAt { get; private set; }

    /// <summary>PostgreSQL xmin: optimistic concurrency token (ADR-002).</summary>
    public uint Version { get; private set; }

    public IReadOnlyList<RequestHistoryEntry> History => _history;
    public IReadOnlyList<RequestNote> Notes => _notes;

    public IRequestState State => RequestStates.For(Status);

    /// <summary>Creates a new Submitted request with its initial history entry.</summary>
    public static Request Submit(long categoryId, string location, string description, long requesterId, DateTime nowUtc)
    {
        if (categoryId <= 0) throw new ArgumentOutOfRangeException(nameof(categoryId));
        ArgumentException.ThrowIfNullOrWhiteSpace(location);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (requesterId <= 0) throw new ArgumentOutOfRangeException(nameof(requesterId));

        var request = new Request
        {
            CategoryId = categoryId,
            Location = location.Trim(),
            Description = description.Trim(),
            RequesterId = requesterId,
            Status = RequestStatus.Submitted,
            CreatedAt = nowUtc
        };
        request._history.Add(RequestHistoryEntry.Create(null, RequestStatus.Submitted, requesterId, nowUtc));
        return request;
    }

    /// <summary>Rebuilds a request that already exists (persistence and tests). Does not add history.</summary>
    public static Request Restore(long id, long categoryId, string location, string description, long requesterId,
        RequestStatus status, long? assignedStaffId, DateTime createdAtUtc, uint version = 0) => new()
        {
            Id = id,
            CategoryId = categoryId,
            Location = location,
            Description = description,
            RequesterId = requesterId,
            Status = status,
            AssignedStaffId = assignedStaffId,
            CreatedAt = createdAtUtc,
            Version = version
        };

    /// <summary>
    /// Gives a new request the id the database generated. Allowed once: an existing identity can never change.
    /// (EF Core sets Id itself; in-memory repositories, such as the test fake, call this.)
    /// </summary>
    public void AssignIdentity(long id)
    {
        if (Id != 0) throw new InvalidOperationException("This request already has an id.");
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        Id = id;
    }

    public virtual bool CanTransitionTo(RequestStatus next) => State.CanTransitionTo(next);

    /// <summary>FR-010: Submitted to Assigned, recording who the request now belongs to.</summary>
    public Result Assign(long assigneeId, long actorId, string? note, DateTime nowUtc)
    {
        if (assigneeId <= 0) return Result.Failure(RequestErrors.AssigneeRequired);
        if (!CanTransitionTo(RequestStatus.Assigned))
            return Result.Failure(RequestErrors.InvalidTransition(Status, RequestStatus.Assigned));

        AssignedStaffId = assigneeId;
        Apply(RequestStatus.Assigned, actorId, note, nowUtc);
        return Result.Success();
    }

    /// <summary>
    /// FR-011: moves the request to <paramref name="next"/> if the current state allows it.
    /// Assigned is refused here because it needs an assignee; use <see cref="Assign"/>.
    /// The assignee is NOT changed by later transitions, so resolving a request never steals it.
    /// </summary>
    public virtual Result TransitionTo(RequestStatus next, long actorId, string? note, DateTime? nowUtc = null)
    {
        if (!CanTransitionTo(next))
            return Result.Failure(RequestErrors.InvalidTransition(Status, next));
        if (next == RequestStatus.Assigned && AssignedStaffId is null)
            return Result.Failure(RequestErrors.UseAssign);

        Apply(next, actorId, note, nowUtc ?? DateTime.UtcNow);
        return Result.Success();
    }

    /// <summary>FR-012: a note that does not change the status. Closed requests are read-only.</summary>
    public Result AddNote(long authorId, string note, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(note))
            return Result.Failure(RequestErrors.Validation("A note cannot be empty."));
        if (Status == RequestStatus.Closed)
            return Result.Failure(RequestErrors.Validation("A closed request cannot be changed."));

        _notes.Add(RequestNote.Create(authorId, note.Trim(), nowUtc));
        return Result.Success();
    }

    private void Apply(RequestStatus next, long actorId, string? note, DateTime nowUtc)
    {
        var previous = Status;
        Status = next;
        _history.Add(RequestHistoryEntry.Create(previous, next, actorId, nowUtc));
        if (!string.IsNullOrWhiteSpace(note))
            _notes.Add(RequestNote.Create(actorId, note.Trim(), nowUtc));
    }
}