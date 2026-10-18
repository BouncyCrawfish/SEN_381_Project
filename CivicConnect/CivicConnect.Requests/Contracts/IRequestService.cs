using CivicConnect.Common;

namespace CivicConnect.Requests.Contracts;

/// Requests module public interface (README "API / Module Interfaces"). Every method checks the
/// signed-in user's permission itself, so the UI hiding a button is never the only protection.
public interface IRequestService
{
    Task<Result<IReadOnlyList<CategoryDto>>> ListCategoriesAsync(CancellationToken ct = default);
    Task<Result<long>> SubmitRequestAsync(SubmitRequestDto dto, CancellationToken ct = default);
    Task<Result<IReadOnlyList<RequestDto>>> GetMyRequestsAsync(CancellationToken ct = default);
    Task<Result<IReadOnlyList<RequestDto>>> GetStaffQueueAsync(RequestQueueFilter? filter = null, CancellationToken ct = default);
    //Manager view: every request including closed ones (ViewAllRequests).
    Task<Result<IReadOnlyList<RequestDto>>> GetAllRequestsAsync(RequestQueueFilter? filter = null, CancellationToken ct = default);
    //Manager report: counts by status and category (ViewReports).
    Task<Result<RequestSummaryDto>> GetSummaryAsync(CancellationToken ct = default);
    Task<Result<RequestDetailDto>> GetRequestDetailAsync(long requestId, CancellationToken ct = default);
    Task<Result> AssignRequestAsync(AssignRequestDto dto, CancellationToken ct = default);
    Task<Result> UpdateRequestStatusAsync(UpdateRequestStatusDto dto, CancellationToken ct = default);
    Task<Result> AddNoteAsync(AddNoteDto dto, CancellationToken ct = default);
}
