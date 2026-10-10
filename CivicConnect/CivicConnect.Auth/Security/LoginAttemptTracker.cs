namespace CivicConnect.Auth;

internal interface ILoginAttemptTracker
{
    bool IsLockedOut(string key, out TimeSpan remaining);
    void RecordFailure(string key);
    void Reset(string key);
}

/// <summary>
/// Slows online password guessing: 5 failures locks that email for 5 minutes.
/// LIMITATION: state lives in this process only. A tampered or restarted client resets it, and the
/// schema has no failed-attempt columns. A durable lockout needs a change request (see README).
/// </summary>
internal sealed class InMemoryLoginAttemptTracker(TimeProvider time) : ILoginAttemptTracker
{
    public const int MaxFailures = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    private sealed class Entry { public int Failures; public DateTimeOffset? LockedUntil; }

    private readonly object _gate = new();
    private readonly Dictionary<string, Entry> _entries = new();

    public bool IsLockedOut(string key, out TimeSpan remaining)
    {
        lock (_gate)
        {
            remaining = TimeSpan.Zero;
            if (!_entries.TryGetValue(key, out var entry) || entry.LockedUntil is null) return false;

            var left = entry.LockedUntil.Value - time.GetUtcNow();
            if (left <= TimeSpan.Zero)
            {
                _entries.Remove(key);
                return false;
            }
            remaining = left;
            return true;
        }
    }

    public void RecordFailure(string key)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(key, out var entry))
                _entries[key] = entry = new Entry();

            entry.Failures++;
            if (entry.Failures >= MaxFailures)
                entry.LockedUntil = time.GetUtcNow() + LockoutDuration;
        }
    }

    public void Reset(string key)
    {
        lock (_gate) _entries.Remove(key);
    }
}
