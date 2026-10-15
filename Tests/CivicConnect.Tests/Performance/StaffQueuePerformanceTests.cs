using System.Diagnostics;
using CivicConnect.AuditLog;
using CivicConnect.Auth;
using CivicConnect.Notifications;
using CivicConnect.Requests;
using CivicConnect.Requests.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.Tests.Performance;

/// TC-21: small load test of the staff queue (NFR-002, NFR-007). Runs the REAL modules against a real
/// PostgreSQL database, signed in as a dev Staff user, and times IRequestService.GetStaffQueueAsync.
///
/// Workload: the open requests in the database (seed them with db/perf/perf_seed_requests.sql),
/// 3 warm-up calls, then 20 measured calls one after another. Target: 95th percentile within 2 s,
/// the NFR-002 placeholder from DEC-002, and no failed calls.
///
/// Skipped unless CIVICCONNECT_TEST_CONNECTION is set (civicconnect_app role), so CI never runs it.
/// Optional: CIVICCONNECT_PERF_STAFF_EMAIL / CIVICCONNECT_PERF_STAFF_PASSWORD (defaults: dev seed staff1).
/// What it does NOT prove: concurrent users, network latency to a hosted database, or UI rendering time.
public sealed class StaffQueuePerformanceTests(ITestOutputHelper output)
{
    private static readonly string? Conn = Environment.GetEnvironmentVariable("CIVICCONNECT_TEST_CONNECTION");
    private const int WarmUpRuns = 3;
    private const int MeasuredRuns = 20;
    private const double TargetP95Milliseconds = 2000;   // NFR-002 placeholder (DEC-002)

    [Fact]
    public async Task Staff_queue_loads_within_the_NFR002_target()
    {
        Assert.SkipWhen(Conn is null, "Set CIVICCONNECT_TEST_CONNECTION (civicconnect_app role) to run the load test.");
        var ct = TestContext.Current.CancellationToken;

        var email = Environment.GetEnvironmentVariable("CIVICCONNECT_PERF_STAFF_EMAIL") ?? "staff1@civicconnect.test";
        var password = Environment.GetEnvironmentVariable("CIVICCONNECT_PERF_STAFF_PASSWORD") ?? "Staff-Test-1234";

        await using var provider = new ServiceCollection()
            .AddNotificationsModule()          // silent notification service: nothing is shown
            .AddAuthModule(Conn!)
            .AddRequestsModule(Conn!)
            .AddAuditLogModule(Conn!)          // provides the domain-event dispatcher RequestService needs
            .BuildServiceProvider();

        var login = await provider.GetRequiredService<IAuthService>().LoginAsync(new LoginDto(email, password), ct);
        Assert.True(login.IsSuccess, "Sign-in as the staff test user failed. Run db/05_dev_seed_users.sql or set the PERF variables.");

        var requests = provider.GetRequiredService<IRequestService>();

        var rows = 0;
        for (var i = 0; i < WarmUpRuns; i++)
        {
            var warm = await requests.GetStaffQueueAsync(null, ct);
            Assert.True(warm.IsSuccess, "Warm-up call failed.");
            rows = warm.Value.Count;
        }

        var timings = new List<double>(MeasuredRuns);
        var failures = 0;
        for (var i = 0; i < MeasuredRuns; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await requests.GetStaffQueueAsync(null, ct);
            stopwatch.Stop();

            if (result.IsFailure) failures++;
            timings.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        timings.Sort();
        double Percentile(double p) => timings[Math.Max(0, (int)Math.Ceiling(p * timings.Count) - 1)];

        var report = string.Join(Environment.NewLine,
            $"TC-21 staff queue load test, {DateTimeOffset.Now:yyyy-MM-dd HH:mm zzz}",
            $"Rows returned per call : {rows}",
            $"Measured calls         : {MeasuredRuns} (after {WarmUpRuns} warm-up)",
            $"Failed calls           : {failures}",
            $"Min / median / p95 / max (ms): {timings[0]:F1} / {Percentile(0.50):F1} / {Percentile(0.95):F1} / {timings[^1]:F1}",
            $"Target                 : p95 <= {TargetP95Milliseconds} ms (NFR-002 placeholder, DEC-002)");

        // Also saved next to the test DLL, because test output is not always shown for passing tests.
        var reportPath = Path.Combine(AppContext.BaseDirectory, "perf-tc21-result.txt");
        await File.WriteAllTextAsync(reportPath, report + Environment.NewLine, ct);
        output.WriteLine(report);
        output.WriteLine($"Saved to: {reportPath}");

        Assert.Equal(0, failures);
        Assert.True(Percentile(0.95) <= TargetP95Milliseconds,
            $"p95 was {Percentile(0.95):F1} ms, above the {TargetP95Milliseconds} ms target.");
    }
}
