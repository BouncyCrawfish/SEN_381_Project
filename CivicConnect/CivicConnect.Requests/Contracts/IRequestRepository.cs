using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Contracts;

/// Raised by the repository when xmin shows someone else changed the row first (ADR-002).
public sealed class RequestConcurrencyException(Exception? inner = null)
    : Exception("The request was modified by another user.", inner);

///Public so the test project can fake it without InternalsVisibleTo.
public interface IRequestRepository
{
    ///Loads the request with its history and notes, or null.
    Task<Request?> FindByIdAsync(long id, CancellationToken ct);

    /// Inserts a new request AND its initial history row in one transaction.
    Task AddAsync(Request request, CancellationToken ct);

    /// Saves the status/assignee change plus any new history/notes in one transaction.
    Task UpdateAsync(Request request, CancellationToken ct);

    Task<IReadOnlyList<RequestCategory>> ListActiveCategoriesAsync(CancellationToken ct);
    Task<RequestCategory?> FindCategoryAsync(long id, CancellationToken ct);

    Task<IReadOnlyList<RequestDto>> ListByRequesterAsync(long requesterId, CancellationToken ct);
    Task<IReadOnlyList<RequestDto>> ListAllAsync(RequestQueueFilter filter, CancellationToken ct);
    Task<RequestSummaryDto> GetSummaryAsync(CancellationToken ct);
    Task<IReadOnlyList<RequestDto>> ListQueueAsync(RequestQueueFilter filter, long? assignedStaffId, CancellationToken ct);
}
