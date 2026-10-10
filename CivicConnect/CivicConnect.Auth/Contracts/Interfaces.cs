using CivicConnect.Common;

namespace CivicConnect.Auth;

/// <summary>Sign-in, sign-out and password change for the person using this desktop client.</summary>
public interface IAuthService
{
    Task<Result<UserDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    void Logout();
    Task<Result> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct = default);
}

/// <summary>Account and role lookups (README section "API / Module Interfaces").</summary>
public interface IUserService
{
    /// <summary>Self-registration. Always creates a Requester; no role can be supplied.</summary>
    Task<Result<UserDto>> RegisterRequesterAsync(RegisterRequesterDto dto, CancellationToken ct = default);

    /// <summary>A user may read themself; Staff and Manager may read anyone.</summary>
    Task<Result<UserDto>> GetByIdAsync(long id, CancellationToken ct = default);

    /// <summary>Staff directory, e.g. to populate an "assign to" list.</summary>
    Task<Result<IReadOnlyList<UserDto>>> ListStaffAsync(CancellationToken ct = default);
}

/// <summary>Who is signed in right now. Read-only for every other module.</summary>
public interface ISessionContext
{
    UserDto? CurrentUser { get; }
    bool IsAuthenticated { get; }
}

/// <summary>Role and permission checks. Fails closed: no session means no access.</summary>
public interface IAccessControl
{
    bool HasPermission(Permission permission);
    bool IsInRole(params UserRole[] roles);
    Result Require(Permission permission);
    Result RequireRole(params UserRole[] roles);
}
