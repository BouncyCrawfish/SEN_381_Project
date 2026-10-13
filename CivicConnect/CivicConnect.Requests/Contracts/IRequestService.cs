using CivicConnect.Common;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Contracts;

/// <summary>
/// Requests model public inteface. Every method checks the 
/// signed-in user's permission itself, so th eUI hiding a button is never the only option
/// </summary>
public interface IRequestService
{
    Task<Result<IReadOnlyList<CategoryDto>>> ListCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<long>> SubmitRequestAsync(SubmitRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<RequestDto>>> GetMyRequestsAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<RequestDto>>> GetStaffQueueAsync(RequestQueueFilter? filter,CancellationToken cancellationToken = default);
    Task<Result<RequestDetailDto>> GetRequestDetailAsync(long requestId, CancellationToken cancellationToken = default);
    Task<Result> AssignRequestAsync(AssignRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateRequestStatusAsync(UpdateRequestStatusDto dto, CancellationToken cancellationToken = default);
    Task<Result> AddNoteAsync(AddNoteDto dto, CancellationToken cancellationToken = default);
}