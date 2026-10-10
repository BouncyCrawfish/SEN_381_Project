using CivicConnect.Common;

namespace CivicConnect.Auth;

/// <summary>
/// Maps to the "users" table in 03_schema.sql. Internal on purpose: other modules get
/// <see cref="UserDto"/>, so the password hash can never leak across a module boundary.
/// </summary>
internal sealed class User
{
    private User() { } // EF Core

    public long Id { get; internal set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;   // stored trimmed + lower-case
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Self-registration can only ever produce a Requester (db/README section 5).</summary>
    public static User CreateRequester(string name, string email, string passwordHash) =>
        Create(name, email, passwordHash, UserRole.Requester);

    // Staff/Manager accounts are provisioned by the owner role, never by the running app.
    // This overload exists for tests only.
    internal static User Create(string name, string email, string passwordHash, UserRole role) => new()
    {
        Name = name,
        Email = email,
        PasswordHash = passwordHash,
        Role = role
    };

    // Production code updates the hash with a single UPDATE (see EfUserRepository); this is for fakes.
    internal void ReplacePasswordHash(string passwordHash) => PasswordHash = passwordHash;
}

internal static class UserMapping
{
    public static UserDto ToDto(this User user) => new(user.Id, user.Name, user.Email, user.Role);
}

internal static class EmailNormalizer
{
    public static string Normalize(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
