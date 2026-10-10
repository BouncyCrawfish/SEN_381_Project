using CivicConnect.Common;

namespace CivicConnect.Auth.Tests;

public sealed class AccessControlTests
{
    [Theory]
    [InlineData(UserRole.Requester, Permission.SubmitRequest, true)]
    [InlineData(UserRole.Requester, Permission.ViewOwnRequests, true)]
    [InlineData(UserRole.Requester, Permission.AssignRequest, false)]
    [InlineData(UserRole.Requester, Permission.ViewReports, false)]
    [InlineData(UserRole.Requester, Permission.ViewStaffDirectory, false)]
    [InlineData(UserRole.Staff, Permission.AssignRequest, true)]
    [InlineData(UserRole.Staff, Permission.UpdateRequestStatus, true)]
    [InlineData(UserRole.Staff, Permission.SubmitRequest, false)]
    [InlineData(UserRole.Staff, Permission.ViewReports, false)]
    [InlineData(UserRole.Manager, Permission.ViewReports, true)]
    [InlineData(UserRole.Manager, Permission.ViewAllRequests, true)]
    [InlineData(UserRole.Manager, Permission.UpdateRequestStatus, false)]
    [InlineData(UserRole.Manager, Permission.SubmitRequest, false)]
    public void Role_permission_matrix(UserRole role, Permission permission, bool expected) =>
        Assert.Equal(expected, RolePermissions.Has(role, permission));

    [Fact]
    public void Nobody_signed_in_means_no_access()
    {
        var h = new AuthHarness();
        Assert.False(h.Access.HasPermission(Permission.SubmitRequest));
        Assert.Equal(ErrorType.Unauthorized, h.Access.Require(Permission.SubmitRequest).Error.Type);
        Assert.Equal(ErrorType.Unauthorized, h.Access.RequireRole(UserRole.Manager).Error.Type);
    }

    [Fact]
    public async Task Signed_in_user_without_the_permission_is_forbidden()
    {
        var h = new AuthHarness();
        await h.SignInAs("r@example.com", "password-12345", UserRole.Requester);

        Assert.True(h.Access.Require(Permission.SubmitRequest).IsSuccess);
        Assert.Equal(ErrorType.Forbidden, h.Access.Require(Permission.AssignRequest).Error.Type);
        Assert.Equal(ErrorType.Forbidden, h.Access.RequireRole(UserRole.Staff, UserRole.Manager).Error.Type);
        Assert.True(h.Access.IsInRole(UserRole.Requester));
    }

    [Fact]
    public async Task Logging_out_removes_access_immediately()
    {
        var h = new AuthHarness();
        await h.SignInAs("s@example.com", "password-12345", UserRole.Staff);
        Assert.True(h.Access.HasPermission(Permission.AssignRequest));

        h.Auth.Logout();
        Assert.False(h.Access.HasPermission(Permission.AssignRequest));
    }
}
