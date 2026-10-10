using CivicConnect.Common;

namespace CivicConnect.Auth.Tests;

public sealed class AuthServiceTests
{
    private const string Pw = "correct horse battery";
    private readonly AuthHarness _h = new();

    [Fact]
    public async Task Valid_login_signs_in_with_the_stored_role_and_ignores_email_case_and_spaces()
    {
        _h.Seed("Sam@Example.com", Pw, UserRole.Staff);

        var result = await _h.Auth.LoginAsync(new LoginDto("  sam@example.COM ", Pw));

        Assert.True(result.IsSuccess);
        Assert.Equal(UserRole.Staff, _h.Session.CurrentUser!.Role);
    }

    [Fact]
    public async Task Wrong_password_and_unknown_email_give_the_same_error_and_no_session()
    {
        _h.Seed("sam@example.com", Pw);

        var wrongPassword = await _h.Auth.LoginAsync(new LoginDto("sam@example.com", "not-the-password"));
        var unknownEmail = await _h.Auth.LoginAsync(new LoginDto("ghost@example.com", Pw));

        Assert.Equal(AuthErrors.InvalidCredentials, wrongPassword.Error);
        Assert.Equal(AuthErrors.InvalidCredentials, unknownEmail.Error);
        Assert.False(_h.Session.IsAuthenticated);
    }

    [Fact]
    public async Task Empty_fields_are_rejected_before_any_lookup()
    {
        var result = await _h.Auth.LoginAsync(new LoginDto("", ""));
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task Five_failures_lock_the_account_even_for_the_correct_password_until_the_lockout_expires()
    {
        _h.Seed("sam@example.com", Pw);
        for (var i = 0; i < InMemoryLoginAttemptTracker.MaxFailures; i++)
            await _h.Auth.LoginAsync(new LoginDto("sam@example.com", "wrong-guess-" + i));

        var locked = await _h.Auth.LoginAsync(new LoginDto("sam@example.com", Pw));
        Assert.Equal(ErrorType.Locked, locked.Error.Type);
        Assert.False(_h.Session.IsAuthenticated);

        _h.Clock.Advance(InMemoryLoginAttemptTracker.LockoutDuration + TimeSpan.FromSeconds(1));
        var afterWait = await _h.Auth.LoginAsync(new LoginDto("sam@example.com", Pw));
        Assert.True(afterWait.IsSuccess);
    }

    [Fact]
    public async Task Successful_login_clears_earlier_failures()
    {
        _h.Seed("sam@example.com", Pw);
        for (var i = 0; i < InMemoryLoginAttemptTracker.MaxFailures - 1; i++)
            await _h.Auth.LoginAsync(new LoginDto("sam@example.com", "wrong-guess-" + i));
        await _h.Auth.LoginAsync(new LoginDto("sam@example.com", Pw));
        _h.Auth.Logout();

        var again = await _h.Auth.LoginAsync(new LoginDto("sam@example.com", "one-more-wrong"));
        Assert.Equal(AuthErrors.InvalidCredentials, again.Error); // not Locked
    }

    [Fact]
    public async Task ChangePassword_requires_sign_in()
    {
        var result = await _h.Auth.ChangePasswordAsync(new ChangePasswordDto(Pw, "a-brand-new-password"));
        Assert.Equal(ErrorType.Unauthorized, result.Error.Type);
    }

    [Fact]
    public async Task ChangePassword_with_wrong_current_password_is_rejected_and_nothing_changes()
    {
        await _h.SignInAs("sam@example.com", Pw, UserRole.Requester);
        var before = _h.Repo.All.Single().PasswordHash;

        var result = await _h.Auth.ChangePasswordAsync(new ChangePasswordDto("nope-nope-nope", "a-brand-new-password"));

        Assert.Equal(AuthErrors.WrongCurrentPassword, result.Error);
        Assert.Equal(before, _h.Repo.All.Single().PasswordHash);
    }

    [Fact]
    public async Task ChangePassword_success_replaces_the_hash_and_the_new_password_works()
    {
        await _h.SignInAs("sam@example.com", Pw, UserRole.Requester);

        var result = await _h.Auth.ChangePasswordAsync(new ChangePasswordDto(Pw, "a-brand-new-password"));
        Assert.True(result.IsSuccess);

        _h.Auth.Logout();
        Assert.False((await _h.Auth.LoginAsync(new LoginDto("sam@example.com", Pw))).IsSuccess);
        Assert.True((await _h.Auth.LoginAsync(new LoginDto("sam@example.com", "a-brand-new-password"))).IsSuccess);
    }

    [Fact]
    public async Task ChangePassword_rejects_a_weak_or_unchanged_new_password()
    {
        await _h.SignInAs("sam@example.com", Pw, UserRole.Requester);
        Assert.Equal(ErrorType.Validation, (await _h.Auth.ChangePasswordAsync(new ChangePasswordDto(Pw, "short"))).Error.Type);
        Assert.Equal(ErrorType.Validation, (await _h.Auth.ChangePasswordAsync(new ChangePasswordDto(Pw, Pw))).Error.Type);
    }

    [Fact]
    public void Dto_ToString_never_prints_a_password()
    {
        var text = new LoginDto("a@example.com", "super-secret-value").ToString()
                 + new RegisterRequesterDto("A", "a@example.com", "super-secret-value")
                 + new ChangePasswordDto("super-secret-value", "super-secret-value2");
        Assert.DoesNotContain("super-secret-value", text);
    }
}
