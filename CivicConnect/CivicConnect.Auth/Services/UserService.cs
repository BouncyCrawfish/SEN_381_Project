using CivicConnect.Common;
using FluentValidation;

namespace CivicConnect.Auth;

internal sealed class UserService(
    IUserRepository users,
    IPasswordHasher hasher,
    ISessionContext session,
    IAccessControl access,
    IValidator<RegisterRequesterDto> registerValidator) : IUserService
{
    public async Task<Result<UserDto>> RegisterRequesterAsync(RegisterRequesterDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var input = dto with
        {
            Name = (dto.Name ?? string.Empty).Trim(),
            Email = EmailNormalizer.Normalize(dto.Email)
        };

        var validation = await registerValidator.ValidateAsync(input, ct);
        if (!validation.IsValid) return Result<UserDto>.Failure(validation.ToError());

        if (await users.EmailExistsAsync(input.Email, ct))
            return Result<UserDto>.Failure(AuthErrors.EmailTaken);

        var hash = await Task.Run(() => hasher.Hash(input.Password), ct);

        // The role is hard-wired: nothing the caller sends can create a Staff or Manager.
        var user = User.CreateRequester(input.Name, input.Email, hash);
        if (!await users.TryAddAsync(user, ct))
            return Result<UserDto>.Failure(AuthErrors.EmailTaken);

        return Result<UserDto>.Success(user.ToDto());
    }

    public async Task<Result<UserDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var current = session.CurrentUser;
        if (current is null) return Result<UserDto>.Failure(AuthErrors.NotAuthenticated);

        // Checked before the lookup so a Requester cannot probe which ids exist.
        if (current.Id != id && current.Role == UserRole.Requester)
            return Result<UserDto>.Failure(AuthErrors.Forbidden);

        var user = await users.FindByIdAsync(id, ct);
        return user is null
            ? Result<UserDto>.Failure(AuthErrors.UserNotFound)
            : Result<UserDto>.Success(user.ToDto());
    }

    public async Task<Result<IReadOnlyList<UserDto>>> ListStaffAsync(CancellationToken ct = default)
    {
        var check = access.Require(Permission.ViewStaffDirectory);
        if (check.IsFailure) return Result<IReadOnlyList<UserDto>>.Failure(check.Error);

        var staff = await users.ListByRoleAsync(UserRole.Staff, ct);
        return Result<IReadOnlyList<UserDto>>.Success(staff.Select(u => u.ToDto()).ToList());
    }
}
