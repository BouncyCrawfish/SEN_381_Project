using System.Reflection;
using System.Runtime.ExceptionServices;
using CivicConnect.Common;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.AuditLog.Events;

internal sealed class DomainEventDispatcher(IServiceProvider services) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handle = handlerType.GetMethod("HandleAsync")!;

        foreach (var handler in services.GetServices(handlerType))
        {
            try
            {
                await (Task)handle.Invoke(handler, [domainEvent, ct])!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); // surface the real exception
            }
        }
    }
}