using Npgsql;
using System;
using System.Threading.Tasks;

namespace CivicConnect.AuditLog.Tests;

public class HistoryImmutabilityTests
{
    private static readonly string? Conn = Environment.GetEnvironmentVariable("CIVICCONNECT_TEST_CONNECTION");

    [Theory]
    [InlineData("UPDATE request_history SET new_status = 'Closed' WHERE id = -1")]
    [InlineData("DELETE FROM request_history WHERE id = -1")]
    public async Task App_role_cannot_update_or_delete_history(string sql)
    {
        Assert.SkipWhen(Conn is null, "Set CIVICCONNECT_TEST_CONNECTION (civicconnect_app role) to run.");

        await using var conn = new NpgsqlConnection(Conn);
        await conn.OpenAsync(TestContext.Current.CancellationToken);
        await using var cmd = new NpgsqlCommand(sql, conn);

        var ex = await Assert.ThrowsAsync<PostgresException>(() => cmd.ExecuteNonQueryAsync(TestContext.Current.CancellationToken));
        Assert.Equal("42501", ex.SqlState); // insufficient_privilege
    }
}