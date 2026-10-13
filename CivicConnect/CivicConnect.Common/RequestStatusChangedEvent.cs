using System;

namespace CivicConnect.Common;

/// Statuses are strings because the RequestStatus enum lives in the Requests module,
/// and Common must not reference it. OldStatus is null for the initial entry.
public sealed record RequestStatusChangedEvent(
    long RequestId,
    string? OldStatus,
    string NewStatus,
    long ChangedBy,
    DateTimeOffset OccurredAt) : IDomainEvent;