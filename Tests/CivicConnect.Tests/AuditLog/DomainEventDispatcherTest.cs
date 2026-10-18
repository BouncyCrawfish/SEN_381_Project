using CivicConnect.AuditLog.Events;
using CivicConnect.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Tests;

public class DomainEventDispatcherTests
{
    private sealed class RecordingHandler : IDomainEventHandler<RequestStatusChangedEvent>
    {
        public int Calls;
        public Task HandleAsync(RequestStatusChangedEvent e, CancellationToken ct = default) { Calls++; return Task.CompletedTask; }
    }

    private sealed class ThrowingHandler : IDomainEventHandler<RequestStatusChangedEvent>
    {
        public Task HandleAsync(RequestStatusChangedEvent e, CancellationToken ct = default) =>
            throw new InvalidOperationException("boom");
    }

    private static RequestStatusChangedEvent Evt() => new(1, null, "Submitted", 1, DateTimeOffset.UtcNow);

    [Fact]
    public async Task Dispatch_calls_every_registered_handler()
    {
        var a = new RecordingHandler();
        var b = new RecordingHandler();
        var sp = new ServiceCollection()
            .AddSingleton<IDomainEventHandler<RequestStatusChangedEvent>>(a)
            .AddSingleton<IDomainEventHandler<RequestStatusChangedEvent>>(b)
            .BuildServiceProvider();

        await new DomainEventDispatcher(sp).DispatchAsync(Evt());

        Assert.Equal(1, a.Calls);
        Assert.Equal(1, b.Calls);
    }

    [Fact]
    public async Task Dispatch_with_no_handlers_does_nothing()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        await new DomainEventDispatcher(sp).DispatchAsync(Evt()); // must not throw
    }

    [Fact]
    public async Task Dispatch_surfaces_the_original_exception_not_a_reflection_wrapper()
    {
        var sp = new ServiceCollection()
            .AddSingleton<IDomainEventHandler<RequestStatusChangedEvent>, ThrowingHandler>()
            .BuildServiceProvider();

        await Assert.ThrowsAsync<InvalidOperationException>(() => new DomainEventDispatcher(sp).DispatchAsync(Evt()));
    }
}