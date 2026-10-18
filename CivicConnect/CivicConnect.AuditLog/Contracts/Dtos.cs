using System;

namespace CivicConnect.AuditLog.Contracts;

public sealed record AuditEntryDto(
    long Id, long RequestId, string? OldStatus, string NewStatus,
    long ChangedBy, DateTimeOffset Timestamp);

/// All filters are optional and combined with AND.
public sealed record AuditQueryDto(
    long? RequestId = null, long? ChangedBy = null, string? NewStatus = null,
    DateTimeOffset? From = null, DateTimeOffset? To = null);