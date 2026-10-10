namespace CivicConnect.Auth;

internal interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

/// <summary>
/// bcrypt via BCrypt.Net-Next. "Enhanced" mode pre-hashes the password with SHA-384, which removes
/// bcrypt's silent 72-byte truncation. Output starts with "$2a$", which satisfies the
/// users_hash_format CHECK constraint (NFR-003). A random salt is generated per hash.
/// </summary>
internal sealed class BCryptPasswordHasher(int workFactor = BCryptPasswordHasher.DefaultWorkFactor) : IPasswordHasher
{
    // Each +1 doubles the cost. 12 is roughly a quarter second on a modern PC;
    // re-measure on the team's real machines and record the figure in the ADR.
    public const int DefaultWorkFactor = 12;

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
        catch (Exception)
        {
            // A malformed stored hash must fail closed and never crash or bypass the login flow.
            return false;
        }
    }
}
