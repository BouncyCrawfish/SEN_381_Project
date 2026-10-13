using System.Threading;
using System.Threading.Tasks;

namespace CivicConnect.Common;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}