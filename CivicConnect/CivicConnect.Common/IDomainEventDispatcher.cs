using System.Threading;
using System.Threading.Tasks;

namespace CivicConnect.Common;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default);
}