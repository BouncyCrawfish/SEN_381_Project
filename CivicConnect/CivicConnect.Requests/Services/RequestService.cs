using CivicConnect.Auth;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;
using FluentValidation;

namespace CivicConnect.Requests.Services;

/// Application service for requests. Order of every operation (fail closed):
///   1. permission check (IAccessControl)  2. validate + trim input  3. row-level ownership rule
///   4. domain rule (State Pattern)         5. persist (one transaction).
/// The acting user ALWAYS comes from the session, never from the DTO.
public sealed class RequestService(
    IRequestRepository repository,
    IAccessControl access,
    ISessionContext session,
    IUserService users,
    TimeProvider clock,
    IValidator<SubmitRequestDto> submitValidator,
    IValidator<AssignRequestDto> assignValidator,
    IValidator<UpdateRequestStatusDto> statusValidator,
    IValidator<AddNoteDto> noteValidator,IDomainEventDispatcher events) : IRequestService
{
    private DateTime Now => clock.GetUtcNow().UtcDateTime;

    public async Task<Result<IReadOnlyList<CategoryDto>>> ListCategoriesAsync(CancellationToken ct = default)
    {
        if (session.CurrentUser is null) return Fail<IReadOnlyList<CategoryDto>>(AuthErrorsProxy.NotAuthenticated);
        var categories = await repository.ListActiveCategoriesAsync(ct);
        return Result<IReadOnlyList<CategoryDto>>.Success(
            categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList());
    }

    public async Task<Result<long>> SubmitRequestAsync(SubmitRequestDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var check = access.Require(Permission.SubmitRequest);
        if (check.IsFailure) return Result<long>.Failure(check.Error);

        var input = dto with { Location = (dto.Location ?? "").Trim(), Description = (dto.Description ?? "").Trim() };
        var validation = await submitValidator.ValidateAsync(input, ct);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToError());

        // FR-002: the category must exist and be active (the DB FK alone would accept a retired one).
        var category = await repository.FindCategoryAsync(input.CategoryId, ct);
        if (category is null || !category.IsActive) return Result<long>.Failure(RequestErrors.CategoryInvalid);

        var requesterId = session.CurrentUser!.Id;
        var request = Request.Submit(category.Id, input.Location, input.Description, requesterId, Now);
        await repository.AddAsync(request, ct);
        return Result<long>.Success(request.Id);
    }

    public async Task<Result<IReadOnlyList<RequestDto>>> GetMyRequestsAsync(CancellationToken ct = default)
    {
        var check = access.Require(Permission.ViewOwnRequests);
        if (check.IsFailure) return Fail<IReadOnlyList<RequestDto>>(check.Error);

        // FR-004: filtered by the SESSION user id, so other requesters' rows are never read.
        var list = await repository.ListByRequesterAsync(session.CurrentUser!.Id, ct);
        return Result<IReadOnlyList<RequestDto>>.Success(list);
    }

    public async Task<Result<IReadOnlyList<RequestDto>>> GetStaffQueueAsync(RequestQueueFilter? filter = null, CancellationToken ct = default)
    {
        var check = access.Require(Permission.ViewStaffQueue);
        if (check.IsFailure) return Fail<IReadOnlyList<RequestDto>>(check.Error);

        var prepared = PrepareFilter(filter);
        if (prepared.IsFailure) return Fail<IReadOnlyList<RequestDto>>(prepared.Error);

        long? mine = prepared.Value.AssignedToMeOnly ? session.CurrentUser!.Id : null;
        var list = await repository.ListQueueAsync(prepared.Value, mine, ct);
        return Result<IReadOnlyList<RequestDto>>.Success(list);
    }

    public async Task<Result<IReadOnlyList<RequestDto>>> GetAllRequestsAsync(RequestQueueFilter? filter = null, CancellationToken ct = default)
    {
        var check = access.Require(Permission.ViewAllRequests);
        if (check.IsFailure) return Fail<IReadOnlyList<RequestDto>>(check.Error);

        var prepared = PrepareFilter(filter);
        if (prepared.IsFailure) return Fail<IReadOnlyList<RequestDto>>(prepared.Error);

        var list = await repository.ListAllAsync(prepared.Value, ct);
        return Result<IReadOnlyList<RequestDto>>.Success(list);
    }

    public async Task<Result<RequestSummaryDto>> GetSummaryAsync(CancellationToken ct = default)
    {
        var check = access.Require(Permission.ViewReports);
        if (check.IsFailure) return Fail<RequestSummaryDto>(check.Error);

        return Result<RequestSummaryDto>.Success(await repository.GetSummaryAsync(ct));
    }

    //Validates and cleans a list filter (status in range, search text trimmed and limited).
    private static Result<RequestQueueFilter> PrepareFilter(RequestQueueFilter? filter)
    {
        filter ??= new RequestQueueFilter();
        if (filter.Status is { } s && !Enum.IsDefined(s))
            return Result<RequestQueueFilter>.Failure(RequestErrors.Validation("Choose a valid status."));
        var search = filter.SearchText?.Trim();
        if (search is { Length: > RequestRules.MaxSearchTextLength })
            return Result<RequestQueueFilter>.Failure(RequestErrors.Validation(
                $"Search text must be at most {RequestRules.MaxSearchTextLength} characters."));
        return Result<RequestQueueFilter>.Success(filter with { SearchText = string.IsNullOrEmpty(search) ? null : search });
    }

    public async Task<Result<RequestDetailDto>> GetRequestDetailAsync(long requestId, CancellationToken ct = default)
    {
        var current = session.CurrentUser;
        if (current is null) return Fail<RequestDetailDto>(AuthErrorsProxy.NotAuthenticated);

        var isStaffSide = access.HasPermission(Permission.ViewRequestHistory);   // Staff and Manager
        if (!isStaffSide && !access.HasPermission(Permission.ViewOwnRequests))
            return Fail<RequestDetailDto>(AuthErrorsProxy.Forbidden);

        var request = await repository.FindByIdAsync(requestId, ct);
        // Same answer for "missing" and "not yours" so ids cannot be probed (NFR-008, FR-006).
        if (request is null || (!isStaffSide && request.RequesterId != current.Id))
            return Fail<RequestDetailDto>(RequestErrors.NotFound);

        var category = await repository.FindCategoryAsync(request.CategoryId, ct);
        var dto = ToDto(request, category?.Name ?? "Unknown");

        string? name = null, email = null;
        if (isStaffSide)
        {
            var requester = await users.GetByIdAsync(request.RequesterId, ct);
            if (requester.IsSuccess) { name = requester.Value.Name; email = requester.Value.Email; }
        }

        var history = request.History.OrderBy(h => h.Timestamp)
            .Select(h => new RequestHistoryDto(h.OldStatus?.ToString(), h.NewStatus.ToString(), h.ChangedBy, h.Timestamp)).ToList();
        // Internal staff notes are not shown to requesters.
        var notes = isStaffSide
            ? request.Notes.OrderBy(n => n.Timestamp).Select(n => new RequestNoteDto(n.AuthorId, n.Note, n.Timestamp)).ToList()
            : new List<RequestNoteDto>();

        return Result<RequestDetailDto>.Success(new RequestDetailDto(dto, name, email, history, notes));
    }

    public async Task<Result> AssignRequestAsync(AssignRequestDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var check = access.Require(Permission.AssignRequest);
        if (check.IsFailure) return check;

        var validation = await assignValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result.Failure(validation.ToError());

        // FR-010: only a real Staff account can own a request (not a requester or manager).
        var assignee = await users.GetByIdAsync(dto.StaffId, ct);
        if (assignee.IsFailure || assignee.Value.Role != UserRole.Staff)
            return Result.Failure(RequestErrors.AssigneeInvalid);

        return await ChangeAsync(dto.RequestId,
            (r, actor) => r.Assign(dto.StaffId, actor, dto.Note, Now), ct);
    }

    public async Task<Result> UpdateRequestStatusAsync(UpdateRequestStatusDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // Resolved and Closed have their own permissions (Master Brief section 3).
        var permission = dto.NewStatus switch
        {
            RequestStatus.Resolved => Permission.ResolveRequest,
            RequestStatus.Closed => Permission.CloseRequest,
            _ => Permission.UpdateRequestStatus
        };
        var check = access.Require(permission);
        if (check.IsFailure) return check;

        if (dto.NewStatus == RequestStatus.Assigned) return Result.Failure(RequestErrors.UseAssign);

        var validation = await statusValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result.Failure(validation.ToError());

        return await ChangeAsync(dto.RequestId,
            // Row-level rule: only the assignee progresses the request (FR-007, FR-011).
            (r, actor) => r.AssignedStaffId == actor
                ? r.TransitionTo(dto.NewStatus, actor, dto.Note, Now)
                : Result.Failure(RequestErrors.NotAssignee), ct);
    }

    public async Task<Result> AddNoteAsync(AddNoteDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var check = access.Require(Permission.AddNote);
        if (check.IsFailure) return check;

        var validation = await noteValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result.Failure(validation.ToError());

        return await ChangeAsync(dto.RequestId, (r, actor) => r.AddNote(actor, dto.Note, Now), ct);
    }

    //Load, apply a domain operation as the signed-in user, save once. Maps lost updates to Conflict.
    private async Task<Result> ChangeAsync(long requestId, Func<Request, long, Result> operation, CancellationToken ct)
    {
        var request = await repository.FindByIdAsync(requestId, ct);
        if (request is null) return Result.Failure(RequestErrors.NotFound);

        var actor = session.CurrentUser!.Id;
        var before = request.Status;
        var result = operation(request, actor);
        if (result.IsFailure) return result;

        try
        {
            await repository.UpdateAsync(request, ct);
        }
        catch (RequestConcurrencyException)
        {
            return Result.Failure(RequestErrors.Conflict);
        }

        // Only a real status change is announced (a note on its own is not).
        if (request.Status != before)
            await PublishAsync(new RequestStatusChangedEvent(
                request.Id, before.ToString(), request.Status.ToString(), actor, clock.GetUtcNow()), ct);
        return Result.Success();
    }

    //The change is already saved, so a failing listener must never turn it into a failure (ADR-005).
    private async Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        try { await events.DispatchAsync(domainEvent, ct); }
        catch (Exception) { /* no logging in the project yet; log here once it exists */ }
    }

    private static RequestDto ToDto(Request r, string category) =>
        new(r.Id, category, r.Location, r.Description, r.Status.ToString(), r.RequesterId, r.AssignedStaffId, r.CreatedAt);

    private static Result<T> Fail<T>(Error error) => Result<T>.Failure(error);
}

//AuthErrors is internal to the Auth module, so the same two outcomes are asked of IAccessControl.
internal static class AuthErrorsProxy
{
    public static readonly Error NotAuthenticated =
        new("auth.not_authenticated", "You must sign in to do this.", ErrorType.Unauthorized);
    public static readonly Error Forbidden =
        new("auth.forbidden", "You do not have permission to do this.", ErrorType.Forbidden);
}
