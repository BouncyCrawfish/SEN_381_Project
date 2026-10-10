using CivicConnect.Common;

namespace CivicConnect.Auth;

internal interface ISessionWriter
{
    void SignIn(UserDto user);
    void SignOut();
}

/// <summary>
/// In-memory session for this desktop client (one user at a time). The role is a snapshot taken
/// at login: a role change made by the owner role applies from the next login.
/// </summary>
internal sealed class SessionContext : ISessionContext, ISessionWriter
{
    private volatile UserDto? _current;

    public UserDto? CurrentUser => _current;
    public bool IsAuthenticated => _current is not null;

    public void SignIn(UserDto user) => _current = user;
    public void SignOut() => _current = null;
}

internal sealed class AccessControl(ISessionContext session) : IAccessControl
{
    public bool HasPermission(Permission permission) =>
        session.CurrentUser is { } user && RolePermissions.Has(user.Role, permission);

    public bool IsInRole(params UserRole[] roles) =>
        session.CurrentUser is { } user && roles.Contains(user.Role);

    public Result Require(Permission permission) => session.CurrentUser switch
    {
        null => Result.Failure(AuthErrors.NotAuthenticated),
        { } user when RolePermissions.Has(user.Role, permission) => Result.Success(),
        _ => Result.Failure(AuthErrors.Forbidden)
    };

    public Result RequireRole(params UserRole[] roles) => session.CurrentUser switch
    {
        null => Result.Failure(AuthErrors.NotAuthenticated),
        { } user when roles.Contains(user.Role) => Result.Success(),
        _ => Result.Failure(AuthErrors.Forbidden)
    };
}
