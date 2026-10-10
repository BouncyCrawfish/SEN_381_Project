using CivicConnect.Common;
using FluentValidation;

namespace CivicConnect.Auth;

internal sealed class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    ILoginAttemptTracker attempts,
    ISessionWriter session,
    ISessionContext sessionContext,
    IValidator<LoginDto> loginValidator,
    IValidator<ChangePasswordDto> changePasswordValidator) : IAuthService
{
    // Verified against when the email is unknown, so "no such user" costs the same time as
    // "wrong password" and response time does not reveal which emails have accounts.
    private readonly Lazy<string> _decoyHash = new(() => hasher.Hash(Guid.NewGuid().ToString("N")));

    public async Task<Result<UserDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var input = dto with { Email = EmailNormalizer.Normalize(dto.Email) };

        var validation = await loginValidator.ValidateAsync(input, ct);
        if (!validation.IsValid) return Result<UserDto>.Failure(validation.ToError());

        if (attempts.IsLockedOut(input.Email, out var remaining))
            return Result<UserDto>.Failure(AuthErrors.LockedOut(remaining));

        var user = await users.FindByEmailAsync(input.Email, ct);

        // bcrypt is deliberately slow; keep it off the WinForms UI thread.
        var passwordOk = await Task.Run(
            () => hasher.Verify(input.Password, user?.PasswordHash ?? _decoyHash.Value), ct);

        if (user is null || !passwordOk)
        {
            attempts.RecordFailure(input.Email);
            return Result<UserDto>.Failure(AuthErrors.InvalidCredentials);
        }

        attempts.Reset(input.Email);
        var signedIn = user.ToDto();
        session.SignIn(signedIn);
        return Result<UserDto>.Success(signedIn);
    }

    public void Logout() => session.SignOut();

    public async Task<Result> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var current = sessionContext.CurrentUser;
        if (current is null) return Result.Failure(AuthErrors.NotAuthenticated);

        var validation = await changePasswordValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result.Failure(validation.ToError());

        if (dto.NewPassword == dto.CurrentPassword)
            return Result.Failure(AuthErrors.Validation("The new password must be different from the current password."));

        // Same lockout as login: a hijacked, already-open session must not be able to guess
        // the current password without limit.
        if (attempts.IsLockedOut(current.Email, out var remaining))
            return Result.Failure(AuthErrors.LockedOut(remaining));

        var user = await users.FindByIdAsync(current.Id, ct);
        if (user is null)
        {
            session.SignOut();
            return Result.Failure(AuthErrors.NotAuthenticated);
        }

        var currentOk = await Task.Run(() => hasher.Verify(dto.CurrentPassword, user.PasswordHash), ct);
        if (!currentOk)
        {
            attempts.RecordFailure(current.Email);
            return Result.Failure(AuthErrors.WrongCurrentPassword);
        }

        attempts.Reset(current.Email);
        var newHash = await Task.Run(() => hasher.Hash(dto.NewPassword), ct);
        await users.UpdatePasswordHashAsync(user.Id, newHash, ct);
        return Result.Success();
    }
}
