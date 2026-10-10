using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using CivicConnect.Common;

namespace CivicConnect.Auth;

internal static class ValidationRules
{
    // No Cascade here: it is only valid directly after RuleFor(...), so callers add it.
    public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Email is required.")
            .MaximumLength(AuthRules.MaxEmailLength).WithMessage("Email is too long.")
            .Matches(AuthRules.EmailPattern, RegexOptions.IgnoreCase).WithMessage("Email address is not valid.");

    public static IRuleBuilderOptions<T, string> MustBeValidNewPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Password is required.")
            .MinimumLength(AuthRules.MinPasswordLength)
                .WithMessage($"Password must be at least {AuthRules.MinPasswordLength} characters.")
            .MaximumLength(AuthRules.MaxPasswordLength)
                .WithMessage($"Password must be at most {AuthRules.MaxPasswordLength} characters.");

    public static Error ToError(this ValidationResult result) =>
        AuthErrors.Validation(string.Join(" ", result.Errors.Select(e => e.ErrorMessage).Distinct()));
}

public sealed class RegisterRequesterDtoValidator : AbstractValidator<RegisterRequesterDto>
{
    public RegisterRequesterDtoValidator()
    {
        RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(AuthRules.MaxNameLength).WithMessage($"Name must be at most {AuthRules.MaxNameLength} characters.");
        RuleFor(x => x.Email).Cascade(CascadeMode.Stop).MustBeValidEmail();
        RuleFor(x => x.Password).Cascade(CascadeMode.Stop).MustBeValidNewPassword();
    }
}

public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(AuthRules.MaxEmailLength).WithMessage("Email is too long.");
        // Only presence/length here: do not reveal the password policy to someone guessing.
        RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(AuthRules.MaxPasswordLength).WithMessage("Password is too long.");
    }
}

public sealed class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Current password is required.")
            .MaximumLength(AuthRules.MaxPasswordLength).WithMessage("Current password is too long.");
        RuleFor(x => x.NewPassword).Cascade(CascadeMode.Stop).MustBeValidNewPassword();
    }
}