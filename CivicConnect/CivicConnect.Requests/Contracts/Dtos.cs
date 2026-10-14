using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Contracts;

public sealed record CategoryDto(long Id, string Name);

/// Deliberately has NO requester id: the requester is always the signed-in user, taken from the
/// session inside the service, so a caller cannot submit a request "as" someone else.
public sealed record SubmitRequestDto(long CategoryId, string Location, string Description);

public sealed record RequestDto(
    long Id, string Category, string Location, string Description, string Status,
    long RequesterId, long? AssignedStaffId, DateTime CreatedAt);

public sealed record RequestHistoryDto(string? OldStatus, string NewStatus, long ChangedBy, DateTime Timestamp);
public sealed record RequestNoteDto(long AuthorId, string Note, DateTime Timestamp);

//RequesterName/Email are only filled for staff and managers (NFR-008).
public sealed record RequestDetailDto(
    RequestDto Request, string? RequesterName, string? RequesterEmail,
    IReadOnlyList<RequestHistoryDto> History, IReadOnlyList<RequestNoteDto> Notes);

public sealed record AssignRequestDto(long RequestId, long StaffId, string? Note);
public sealed record UpdateRequestStatusDto(long RequestId, RequestStatus NewStatus, string? Note);
public sealed record AddNoteDto(long RequestId, string Note);

//Manager overview (FR-014 style reporting): counts only, no personal data.
public sealed record RequestSummaryDto(
    int Total, IReadOnlyDictionary<string, int> ByStatus, IReadOnlyDictionary<string, int> ByCategory);

public sealed record RequestQueueFilter(RequestStatus? Status = null, string? SearchText = null, bool AssignedToMeOnly = false);
