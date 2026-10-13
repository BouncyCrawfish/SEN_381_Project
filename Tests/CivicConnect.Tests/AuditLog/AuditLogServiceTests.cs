using CivicConnect.AuditLog.Contracts;
using CivicConnect.AuditLog.Domain;
using CivicConnect.AuditLog.Services;
using CivicConnect.Common;
using System;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Tests;

public class AuditLogServiceTests
{
    private static (AuditLogService svc, FakeAuditLogRepository repo) Make()
    {
        var repo = new FakeAuditLogRepository();
        var now = DateTimeOffset.UtcNow;
        repo.Entries.Add(new AuditEntry(1, null, "Submitted", 10, now));
        repo.Entries.Add(new AuditEntry(1, "Submitted", "Assigned", 20, now));
        repo.Entries.Add(new AuditEntry(2, null, "Submitted", 10, now));
        return (new AuditLogService(repo), repo);
    }

    [Fact]
    public async Task GetHistoryForRequest_returns_only_that_requests_entries()
    {
        var (svc, _) = Make();
        var result = await svc.GetHistoryForRequestAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, e => Assert.Equal(1, e.RequestId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetHistoryForRequest_rejects_invalid_ids(long id)
    {
        var (svc, _) = Make();
        var result = await svc.GetHistoryForRequestAsync(id);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task Query_rejects_from_after_to()
    {
        var (svc, _) = Make();
        var now = DateTimeOffset.UtcNow;
        var result = await svc.QueryAsync(new AuditQueryDto(From: now, To: now.AddDays(-1)));

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Query_filters_by_actor_and_status()
    {
        var (svc, _) = Make();
        var result = await svc.QueryAsync(new AuditQueryDto(ChangedBy: 10, NewStatus: "Submitted"));

        Assert.Equal(2, result.Value.Count);
    }
}