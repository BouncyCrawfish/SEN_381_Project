using CivicConnect.Common;

namespace CivicConnect.Auth.Tests;

public sealed class UserServiceTests
{
    private readonly AuthHarness _h = new();

    [Fact]
    public async Task Registration_creates_a_Requester_with_a_hashed_password()
    {
        var result = await _h.Users.RegisterRequesterAsync(
            new RegisterRequesterDto("  Alex  ", " Alex@Example.com ", "a-strong-password"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserRole.Requester, result.Value.Role);
        var stored = _h.Repo.All.Single();
        Assert.Equal("alex@example.com", stored.Email);
        Assert.Equal("Alex", stored.Name);
        Assert.NotEqual("a-strong-password", stored.PasswordHash);
        Assert.StartsWith("$2", stored.PasswordHash);
    }

    [Fact]
    public async Task Duplicate_email_is_rejected_regardless_of_case()
    {
        await _h.Users.RegisterRequesterAsync(new RegisterRequesterDto("A", "a@example.com", "a-strong-password"), TestContext.Current.CancellationToken);
        var again = await _h.Users.RegisterRequesterAsync(new RegisterRequesterDto("B", "A@EXAMPLE.COM", "another-strong-pw"), TestContext.Current.CancellationToken);

        Assert.Equal(ErrorType.Conflict, again.Error.Type);
        Assert.Single(_h.Repo.All);
    }

    [Theory]
    [InlineData("", "a@example.com", "a-strong-password")]
    [InlineData("A", "not-an-email", "a-strong-password")]
    [InlineData("A", "a@example", "a-strong-password")]
    [InlineData("A", "a@example.com", "short")]
    public async Task Invalid_registration_is_rejected_and_nothing_is_stored(string name, string email, string password)
    {
        var result = await _h.Users.RegisterRequesterAsync(new RegisterRequesterDto(name, email, password), TestContext.Current.CancellationToken);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Empty(_h.Repo.All);
    }

    [Fact]
    public async Task Requester_cannot_list_staff_but_staff_can()
    {
        _h.Seed("staff1@example.com", "password-12345", UserRole.Staff, "Staff One");
        await _h.SignInAs("r@example.com", "password-12345", UserRole.Requester);
        Assert.Equal(ErrorType.Forbidden, (await _h.Users.ListStaffAsync(TestContext.Current.CancellationToken)).Error.Type);

        _h.Auth.Logout();
        await _h.SignInAs("s@example.com", "password-12345", UserRole.Staff);
        var list = await _h.Users.ListStaffAsync(TestContext.Current.CancellationToken);
        Assert.True(list.IsSuccess);
        Assert.All(list.Value, u => Assert.Equal(UserRole.Staff, u.Role));
    }

    [Fact]
    public async Task Requester_can_read_themself_but_not_other_users()
    {
        var other = _h.Seed("other@example.com", "password-12345", UserRole.Staff);
        await _h.SignInAs("r@example.com", "password-12345", UserRole.Requester);
        var me = _h.Session.CurrentUser!;

        Assert.True((await _h.Users.GetByIdAsync(me.Id, TestContext.Current.CancellationToken)).IsSuccess);
        Assert.Equal(ErrorType.Forbidden, (await _h.Users.GetByIdAsync(other.Id, TestContext.Current.CancellationToken)).Error.Type);
    }

    [Fact]
    public async Task Anonymous_caller_cannot_read_users()
    {
        var result = await _h.Users.GetByIdAsync(1, TestContext.Current.CancellationToken);
        Assert.Equal(ErrorType.Unauthorized, result.Error.Type);
    }
}
