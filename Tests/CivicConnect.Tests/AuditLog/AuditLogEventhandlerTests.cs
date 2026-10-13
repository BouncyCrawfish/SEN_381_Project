using CivicConnect.AuditLog.Events;
using CivicConnect.Common;
using System;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Tests;

public class AuditLogEventHandlerTests
{
    [Fact]
    public async Task Handle_appends_one_entry_mapped_from_the_event()
    {
        var repo = new FakeAuditLogRepository();
        var handler = new AuditLogEventHandler(repo);
        var when = new DateTimeOffset(2026, 10, 13, 8, 0, 0, TimeSpan.FromHours(2));

        await handler.HandleAsync(new RequestStatusChangedEvent(7, "Submitted", "Assigned", 3, when));

        var entry = Assert.Single(repo.Entries);
        Assert.Equal(7, entry.RequestId);
        Assert.Equal("Submitted", entry.OldStatus);
        Assert.Equal("Assigned", entry.NewStatus);
        Assert.Equal(3, entry.ChangedBy);
        Assert.Equal(TimeSpan.Zero, entry.Timestamp.Offset);
    }

    [Fact]
    public async Task Handle_allows_null_old_status_for_the_first_entry()
    {
        var repo = new FakeAuditLogRepository();
        await new AuditLogEventHandler(repo)
            .HandleAsync(new RequestStatusChangedEvent(1, null, "Submitted", 2, DateTimeOffset.UtcNow));

        Assert.Null(Assert.Single(repo.Entries).OldStatus);
    }
}