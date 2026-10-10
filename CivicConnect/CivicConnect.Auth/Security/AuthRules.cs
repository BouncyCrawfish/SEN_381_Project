using CivicConnect.Common;

namespace CivicConnect.Auth;

internal static class AuthRules
{
    public const int MaxNameLength = 100;       // users.name VARCHAR(100)
    public const int MaxEmailLength = 254;      // users.email VARCHAR(254)
    public const int MinPasswordLength = 10;
    public const int MaxPasswordLength = 128;   // stops multi-megabyte inputs being fed to bcrypt

    // Same pattern as the users_email_format CHECK, so the DB constraint is a safety net
    // and never the first thing a user hits.
    public const string EmailPattern = @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$";
}

internal static class AuthErrors
{
    // One message for "no such user" and "wrong password" so the login screen cannot be used
    // to discover which emails have accounts.
    public static readonly Error InvalidCredentials =
        new("auth.invalid_credentials", "Invalid email or password.", ErrorType.Unauthorized);
    public static readonly Error NotAuthenticated =
        new("auth.not_authenticated", "You must sign in to do this.", ErrorType.Unauthorized);
    public static readonly Error Forbidden =
        new("auth.forbidden", "You do not have permission to do this.", ErrorType.Forbidden);
    public static readonly Error EmailTaken =
        new("user.email_taken", "An account with this email already exists.", ErrorType.Conflict);
    public static readonly Error UserNotFound =
        new("user.not_found", "User not found.", ErrorType.NotFound);
    public static readonly Error WrongCurrentPassword =
        new("auth.wrong_current_password", "The current password is incorrect.", ErrorType.Validation);

    public static Error Validation(string message) => new("auth.validation", message, ErrorType.Validation);

    public static Error LockedOut(TimeSpan remaining)
    {
        var minutes = Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes));
        return new("auth.locked_out", $"Too many failed attempts. Try again in {minutes} minute(s).", ErrorType.Locked);
    }
}
