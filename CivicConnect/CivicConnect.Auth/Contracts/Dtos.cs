using CivicConnect.Common;

namespace CivicConnect.Auth;

/// <summary>Safe view of a user. Deliberately has no password hash.</summary>
public sealed record UserDto(long Id, string Name, string Email, UserRole Role);

// DTOs that carry passwords override ToString so a password can never end up in a log line
// or a debugger watch window via the compiler-generated record ToString().
public sealed record RegisterRequesterDto(string Name, string Email, string Password)
{
    public override string ToString() => $"RegisterRequesterDto {{ Name = {Name}, Email = {Email}, Password = *** }}";
}

public sealed record LoginDto(string Email, string Password)
{
    public override string ToString() => $"LoginDto {{ Email = {Email}, Password = *** }}";
}

public sealed record ChangePasswordDto(string CurrentPassword, string NewPassword)
{
    public override string ToString() => "ChangePasswordDto { CurrentPassword = ***, NewPassword = *** }";
}
