namespace CivicConnect.Requests.Domain;

///State Pattern (ADR-004). Each state class knows which statuses may follow it, so the legal
///transitions live in exactly one place per state instead of on a big swith.
///Adding new statuses means adding one class, not editing every existing rule.
public interface IRequestState
{
    RequestStatus Status { get; }
    IReadOnlyCollection<RequestStatus> AllowedNext { get; }
    bool CanTransitionTo(RequestStatus next);
}

public abstract class RequestState: IRequestState
{
    private readonly RequestStatus[] _allowedNext;

    protected RequestState(RequestStatus status, params RequestStatus[] allowedNext)
    {
        Status = status;
        _allowedNext = allowedNext;
    }

    public RequestStatus Status { get; }
    public IReadOnlyCollection<RequestStatus> AllowedNext => _allowedNext;
    public bool CanTransitionTo(RequestStatus next) => Array.IndexOf(_allowedNext, next) >= 0;
}

public sealed class SubmittedState() : RequestState(RequestStatus.Submitted, RequestStatus.Assigned);
public sealed class AssignedState() : RequestState(RequestStatus.Assigned, RequestStatus.InProgress);
public sealed class InProgressState() : RequestState(RequestStatus.InProgress, RequestStatus.Resolved);
public sealed class ResolvedState() : RequestState(RequestStatus.Resolved, RequestStatus.Closed);
public sealed class ClosedState() : RequestState(RequestStatus.Closed); //terminal: allows nothing to follow

//Maps a stored status to its (stateless, shared) state object.
public static class RequestStates
{
  private static readonly IReadOnlyDictionary<RequestStatus, IRequestState> Map = new Dictionary<RequestStatus, IRequestState>
  {
    { RequestStatus.Submitted, new SubmittedState() },
    { RequestStatus.Assigned, new AssignedState() },
    { RequestStatus.InProgress, new InProgressState() },
    { RequestStatus.Resolved, new ResolvedState() },
    { RequestStatus.Closed, new ClosedState() }
  };

    public static IRequestState For(RequestStatus status) => Map.TryGetValue(status, out var state) 
        ? state : throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown request status.");
}
