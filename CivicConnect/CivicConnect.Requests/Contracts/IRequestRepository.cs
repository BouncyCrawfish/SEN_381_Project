using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Contracts;

/// <summary>Raised by the repository when xmin shows someone else changed the row first (ADR-002).</summary>
public sealed class RequestConcurrencyException(Exception? inner = null)
    : Exception("The request was modified by another user.", inner);

/// <summary>Public so the test project can fake it without InternalsVisibleTo.</summary>
public interface IRequestRepository
{
    /// <summary>Loads the request with its history and notes, or null.</summary>
    Task<Request?> FindByIdAsync(long id, CancellationToken ct);

    /// <summary>Inserts a new request AND its initial history row in one transaction.</summary>
    Task AddAsync(Request request, CancellationToken ct);

    /// <summary>
    /// Saves the status/assignee change plus any new history/notes in one transaction.
    /// Throws <see cref="RequestConcurrencyException"/> on a lost update.
    /// </summary>
    Task UpdateAsync(Request request, CancellationToken ct);

    Task<IReadOnlyList<RequestCategory>> ListActiveCategoriesAsync(CancellationToken ct);
    Task<RequestCategory?> FindCategoryAsync(long id, CancellationToken ct);

    Task<IReadOnlyList<RequestDto>> ListByRequesterAsync(long requesterId, CancellationToken ct);
    Task<IReadOnlyList<RequestDto>> ListQueueAsync(RequestQueueFilter filter, long? assignedStaffId, CancellationToken ct);
}