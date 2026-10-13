using CivicConnect.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Contracts;

/// Read-only on purpose (NFR-005). Writing happens only through AuditLogEventHandler.
public interface IAuditLogService
{
    Task<Result<IReadOnlyList<AuditEntryDto>>> GetHistoryForRequestAsync(long requestId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<AuditEntryDto>>> QueryAsync(AuditQueryDto query, CancellationToken ct = default);
}