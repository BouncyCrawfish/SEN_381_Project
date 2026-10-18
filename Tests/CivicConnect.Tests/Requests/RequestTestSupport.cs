using CivicConnect.Auth;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;
using CivicConnect.Requests.Services;

namespace CivicConnect.Tests.Requests;

internal sealed class FixedClock : TimeProvider
{
    public DateTimeOffset Now { get; set; } = new(2026, 10, 12, 8, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => Now;
}

internal sealed class FakeEventDispatcher : IDomainEventDispatcher
{
    public List<IDomainEvent> Published { get; } = new();

    /// <summary>Set to simulate a listener that fails (the saved change must still succeed).</summary>
    public bool Throw { get; set; }

    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        if (Throw) throw new InvalidOperationException("listener failed");
        Published.Add(domainEvent);
        return Task.CompletedTask;
    }
}

internal sealed class FakeUserService : IUserService
{
    public List<UserDto> Users { get; } = new();

    public Task<Result<UserDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user is null
            ? Result<UserDto>.Failure(new Error("user.not_found", "User not found.", ErrorType.NotFound))
            : Result<UserDto>.Success(user));
    }

    public Task<Result<IReadOnlyList<UserDto>>> ListStaffAsync(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyList<UserDto>>.Success(Users.Where(u => u.Role == UserRole.Staff).ToList()));

    public Task<Result<UserDto>> RegisterRequesterAsync(RegisterRequesterDto dto, CancellationToken ct = default) =>
        throw new NotSupportedException();
}

internal sealed class FakeRequestRepository : IRequestRepository
{
    private long _nextId = 100;
    public Dictionary<long, Request> Requests { get; } = new();
    public List<RequestCategory> Categories { get; } = new()
    {
        new(1, "Pothole"), new(2, "Streetlight"), new(3, "Retired category", isActive: false)
    };

    /// <summary>Set to simulate another user saving first (xmin mismatch).</summary>
    public bool ThrowConcurrencyOnUpdate { get; set; }
    public int UpdateCalls { get; private set; }

    public Task<Request?> FindByIdAsync(long id, CancellationToken ct) =>
        Task.FromResult(Requests.GetValueOrDefault(id));

    public Task AddAsync(Request request, CancellationToken ct)
    {
        request.AssignIdentity(_nextId++);
        Requests[request.Id] = request;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Request request, CancellationToken ct)
    {
        UpdateCalls++;
        if (ThrowConcurrencyOnUpdate) throw new RequestConcurrencyException();
        Requests[request.Id] = request;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RequestCategory>> ListActiveCategoriesAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<RequestCategory>>(Categories.Where(c => c.IsActive).ToList());

    public Task<RequestCategory?> FindCategoryAsync(long id, CancellationToken ct) =>
        Task.FromResult(Categories.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<RequestDto>> ListByRequesterAsync(long requesterId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<RequestDto>>(
            Requests.Values.Where(r => r.RequesterId == requesterId).Select(ToDto).ToList());

    public Task<IReadOnlyList<RequestDto>> ListQueueAsync(RequestQueueFilter filter, long? assignedStaffId, CancellationToken ct)
    {
        var q = Requests.Values.AsEnumerable();
        q = filter.Status is { } s ? q.Where(r => r.Status == s) : q.Where(r => r.Status != RequestStatus.Closed);
        if (assignedStaffId is { } id) q = q.Where(r => r.AssignedStaffId == id);
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
            q = q.Where(r => r.Description.Contains(filter.SearchText!, StringComparison.OrdinalIgnoreCase)
                          || r.Location.Contains(filter.SearchText!, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IReadOnlyList<RequestDto>>(q.Select(ToDto).ToList());
    }

    // Implemented to satisfy IRequestRepository.ListAllAsync
    public Task<IReadOnlyList<RequestDto>> ListAllAsync(RequestQueueFilter filter, CancellationToken ct)
    {
        var q = Requests.Values.AsEnumerable();
        q = filter.Status is { } s ? q.Where(r => r.Status == s) : q.Where(r => r.Status != RequestStatus.Closed);
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
            q = q.Where(r => r.Description.Contains(filter.SearchText!, StringComparison.OrdinalIgnoreCase)
                          || r.Location.Contains(filter.SearchText!, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IReadOnlyList<RequestDto>>(q.Select(ToDto).ToList());
    }

    public Task<RequestSummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        var statuses = Enum.GetValues<RequestStatus>()
                           .Cast<RequestStatus>()
                           .ToDictionary(s => s.ToString(),
                                         s => Requests.Values.Count(r => r.Status == s));

        var total = statuses.Values.Sum();

        var byCategory = Requests.Values
                                 .GroupBy(r => Categories.First(c => c.Id == r.CategoryId).Name)
                                 .OrderBy(g => g.Key)
                                 .ToDictionary(g => g.Key, g => g.Count());

        return Task.FromResult(new RequestSummaryDto(total, statuses, byCategory));
    }

    private RequestDto ToDto(Request r) =>
        new(r.Id, Categories.First(c => c.Id == r.CategoryId).Name, r.Location, r.Description,
            r.Status.ToString(), r.RequesterId, r.AssignedStaffId, r.CreatedAt);
}
/// <summary>Real service + real validators + real Auth access control, with an in-memory repository.</summary>
internal sealed class RequestHarness
{
    public FakeRequestRepository Repo { get; } = new();
    public FakeUserService Users { get; } = new();
    public FixedClock Clock { get; } = new();
    public FakeEventDispatcher Events { get; } = new();
    public SessionContext Session { get; } = new();
    public RequestService Service { get; }

    public const long RequesterId = 1, OtherRequesterId = 2, StaffId = 10, OtherStaffId = 11, ManagerId = 20;

    public RequestHarness()
    {
        Users.Users.AddRange(new[]
        {
            new UserDto(RequesterId, "Rita Requester", "rita@example.com", UserRole.Requester),
            new UserDto(OtherRequesterId, "Oscar Other", "oscar@example.com", UserRole.Requester),
            new UserDto(StaffId, "Sam Staff", "sam@example.com", UserRole.Staff),
            new UserDto(OtherStaffId, "Sue Staff", "sue@example.com", UserRole.Staff),
            new UserDto(ManagerId, "Mo Manager", "mo@example.com", UserRole.Manager)
        });

        Service = new RequestService(Repo, new AccessControl(Session), Session, Users, Clock,
    new SubmitRequestDtoValidator(), new AssignRequestDtoValidator(),
    new UpdateRequestStatusDtoValidator(), new AddNoteDtoValidator(), Events);
    }

    public void SignInAs(long userId) => Session.SignIn(Users.Users.Single(u => u.Id == userId));
    public void SignOut() => Session.SignOut();

    /// <summary>Puts a request straight into the repository in the given state.</summary>
    public Request Seed(RequestStatus status, long requesterId = RequesterId, long? assignee = null)
    {
        var request = Request.Restore(0, 1, "12 Long Street", "Pothole outside number twelve.", requesterId,
            status, assignee ?? (status == RequestStatus.Submitted ? null : StaffId), Clock.Now.UtcDateTime);
        Repo.AddAsync(request, default).GetAwaiter().GetResult();
        return request;
    }
}
