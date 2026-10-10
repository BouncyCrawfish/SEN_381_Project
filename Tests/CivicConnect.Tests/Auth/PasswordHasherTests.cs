using System.Text.RegularExpressions;

namespace CivicConnect.Auth.Tests;

public sealed class PasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new(4);

    // Exact pattern of the users_hash_format CHECK in db/03_schema.sql (NFR-003).
    private static readonly Regex DbHashCheck = new(@"^\$(2[aby]|argon2(id|i|d))\$");

    [Fact]
    public void Hash_is_never_the_plaintext_and_satisfies_the_database_CHECK()
    {
        var hash = _hasher.Hash("a-long-test-password");
        Assert.NotEqual("a-long-test-password", hash);
        Assert.Matches(DbHashCheck, hash);
    }

    [Fact]
    public void Same_password_hashed_twice_gives_different_hashes_because_of_the_salt() =>
        Assert.NotEqual(_hasher.Hash("same-password-1"), _hasher.Hash("same-password-1"));

    [Fact]
    public void Verify_accepts_the_right_password_and_rejects_the_wrong_one()
    {
        var hash = _hasher.Hash("right-password-1");
        Assert.True(_hasher.Verify("right-password-1", hash));
        Assert.False(_hasher.Verify("wrong-password-1", hash));
    }

    [Fact]
    public void Passwords_differing_only_after_72_bytes_are_not_treated_as_equal()
    {
        var prefix = new string('a', 80);
        var hash = _hasher.Hash(prefix + "1");
        Assert.False(_hasher.Verify(prefix + "2", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("$2a$04$tooshort")]
    public void Malformed_stored_hash_fails_closed_instead_of_throwing(string badHash) =>
        Assert.False(_hasher.Verify("anything-at-all", badHash));

    [Fact]
    public void Production_work_factor_is_at_least_12() =>
        Assert.True(BCryptPasswordHasher.DefaultWorkFactor >= 12);
}
