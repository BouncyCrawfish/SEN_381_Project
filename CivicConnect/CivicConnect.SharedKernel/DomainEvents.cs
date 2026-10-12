namespace CivicConnect.SharedKernel;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}


public sealed record RequestStatusChangedEvent(
    int RequestId,
    RequestStatus? OldStatus,
    RequestStatus NewStatus,
    int ChangedByUserId,
    int RequesterId,
    DateTimeOffset OccurredAt) : IDomainEvent;


public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IPostCommitEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventDispatcher
{
    
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;

    Task DispatchPostCommitAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
}
