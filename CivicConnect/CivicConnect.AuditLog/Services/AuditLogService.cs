using CivicConnect.AuditLog.Contracts;
using CivicConnect.AuditLog.Persistence;
using CivicConnect.Common;

namespace CivicConnect.AuditLog.Services;

internal sealed class AuditLogService(IAuditLogRepository repository) : IAuditLogService
{
    private const int MaxRows = 500; // keeps management queries bounded (NFR-002)

    public Task<Result<IReadOnlyList<AuditEntryDto>>> GetHistoryForRequestAsync(long requestId, CancellationToken ct = default)
    {
        if (requestId <= 0)
            return Task.FromResult(Result<IReadOnlyList<AuditEntryDto>>.Failure(
                new Error("Audit.InvalidRequestId", "A valid request id is required.", ErrorType.Validation)));

        return QueryAsync(new AuditQueryDto(RequestId: requestId), ct);
    }

    public async Task<Result<IReadOnlyList<AuditEntryDto>>> QueryAsync(AuditQueryDto query, CancellationToken ct = default)
    {
        if (query.From > query.To)
            return Result<IReadOnlyList<AuditEntryDto>>.Failure(
                new Error("Audit.InvalidRange", "'From' must not be after 'To'.", ErrorType.Validation));

        var rows = await repository.QueryAsync(query, MaxRows, ct);
        IReadOnlyList<AuditEntryDto> dtos = rows
            .Select(e => new AuditEntryDto(e.Id, e.RequestId, e.OldStatus, e.NewStatus, e.ChangedBy, e.Timestamp))
            .ToList();
        return Result<IReadOnlyList<AuditEntryDto>>.Success(dtos);
    }
}