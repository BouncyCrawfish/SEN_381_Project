using CivicConnect.Common;

namespace CivicConnect.Requests.Domain;

/// <summary>
/// Field limits in ONE place. Lengths that exist in db/03_schema.sql / 04_requests_module.sql are
/// mirrored here so the DB constraint is a safety net and never the first thing a user hits.
/// ASSUMPTION: MinDescriptionLength and MaxDescriptionLength are not stated in the PED; confirm them
/// against FR-001 and update TC-07 if they change.
/// </summary>
public static class RequestRules
{
    public const int MaxLocationLength = 200;      // request.location VARCHAR(200)
    public const int MinDescriptionLength = 10;    // ASSUMPTION (TC-07 boundary)
    public const int MaxDescriptionLength = 1000;  // 04_requests_module.sql CHECK
    public const int MaxNoteLength = 1000;         // 04_requests_module.sql CHECK
    public const int MaxSearchTextLength = 100;
}

public static class RequestErrors
{
    public static readonly Error NotFound =
        new("request.not_found", "Request not found.", ErrorType.NotFound);
    public static readonly Error CategoryInvalid =
        new("request.category_invalid", "Please choose a category from the list.", ErrorType.Validation);
    public static readonly Error AssigneeInvalid =
        new("request.assignee_invalid", "The request can only be assigned to a staff member.", ErrorType.Validation);
    public static readonly Error UseAssign =
        new("request.use_assign", "Use the assign action to assign a request.", ErrorType.Validation);
    public static readonly Error AssigneeRequired =
        new("request.assignee_required", "A request cannot be assigned without a staff member.", ErrorType.Validation);
    public static readonly Error NotAssignee =
        new("request.not_assignee", "Only the staff member this request is assigned to can change its status.", ErrorType.Forbidden);
    public static readonly Error Conflict =
        new("request.conflict", "This request was changed by someone else. Please reload it and try again.", ErrorType.Conflict);

    public static Error Validation(string message) => new("request.validation", message, ErrorType.Validation);

    public static Error InvalidTransition(RequestStatus from, RequestStatus to) =>
        new("request.invalid_transition", $"Invalid state transition from {from} to {to}.", ErrorType.Validation);
}
