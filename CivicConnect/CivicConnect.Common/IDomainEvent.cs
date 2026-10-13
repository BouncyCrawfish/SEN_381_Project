using System;

namespace CivicConnect.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}