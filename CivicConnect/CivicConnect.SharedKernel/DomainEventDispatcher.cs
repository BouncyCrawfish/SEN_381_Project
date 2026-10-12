namespace CivicConnect.SharedKernel;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly Dictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> _transactional = new();
    private readonly Dictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> _postCommit = new();
    private readonly IOperationalLog _log;

    public DomainEventDispatcher(IOperationalLog log)
    {
        _log = log;
    }

    public void Register<TEvent>(IDomainEventHandler<TEvent> handler) where TEvent : IDomainEvent
        => Add(_transactional, typeof(TEvent), (e, ct) => handler.HandleAsync((TEvent)e, ct));

    public void RegisterPostCommit<TEvent>(IPostCommitEventHandler<TEvent> handler) where TEvent : IDomainEvent
        => Add(_postCommit, typeof(TEvent), (e, ct) => handler.HandleAsync((TEvent)e, ct));

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        if (!_transactional.TryGetValue(typeof(TEvent), out var handlers)) return;
        foreach (var handler in handlers.ToArray())
        {
            await handler(domainEvent, cancellationToken);
        }
    }

    public async Task DispatchPostCommitAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        if (!_postCommit.TryGetValue(typeof(TEvent), out var handlers)) return;
        foreach (var handler in handlers.ToArray())
        {
            try
            {
                await handler(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error($"Post-commit handler failed for {typeof(TEvent).Name}.", ex);
            }
        }
    }

    private static void Add(
        Dictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> map,
        Type eventType,
        Func<IDomainEvent, CancellationToken, Task> handler)
    {
        if (!map.TryGetValue(eventType, out var list))
        {
            list = new List<Func<IDomainEvent, CancellationToken, Task>>();
            map[eventType] = list;
        }
        list.Add(handler);
    }
}
