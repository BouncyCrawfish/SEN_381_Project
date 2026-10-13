using CivicConnect.Common;
using CivicConnect.Requests.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace CivicConnect.Requests.Contracts;

internal static class ValidationExtensions
{
    public static Error ToError(this ValidationResult result) =>
        RequestErrors.Validation(string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage).Distinct()));

    public static IRuleBuilderOptions<T, string?> MustBeValidNote<T>(this IRuleBuilder<T, string?> rule) =>
        rule.MaximumLength(RequestRules.MaxNoteLength)
            .WithMessage($"Note must be at most {RequestRules.MaxNoteLength} characters.");
}

public sealed class SubmitRequestDtoValidator : AbstractValidator<SubmitRequestDto>
{
    public SubmitRequestDtoValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Please choose a category.");

        RuleFor(x => x.Location).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(RequestRules.MaxLocationLength)
                .WithMessage($"Location must be at most {RequestRules.MaxLocationLength} characters.");

        RuleFor(x => x.Description).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(RequestRules.MinDescriptionLength)
                .WithMessage($"Description must be at least {RequestRules.MinDescriptionLength} characters.")
            .MaximumLength(RequestRules.MaxDescriptionLength)
                .WithMessage($"Description must be at most {RequestRules.MaxDescriptionLength} characters.");
    }
}

public sealed class AssignRequestDtoValidator : AbstractValidator<AssignRequestDto>
{
    public AssignRequestDtoValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0).WithMessage("Choose a request.");
        RuleFor(x => x.StaffId).GreaterThan(0).WithMessage("Choose a staff member.");
        RuleFor(x => x.Note).MustBeValidNote();
    }
}

public sealed class UpdateRequestStatusDtoValidator : AbstractValidator<UpdateRequestStatusDto>
{
    public UpdateRequestStatusDtoValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0).WithMessage("Choose a request.");
        RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Choose a valid status.");
        RuleFor(x => x.Note).MustBeValidNote();
        // A resolution note is mandatory so the requester always learns what was done (FR-012, FR-005).
        RuleFor(x => x.Note).NotEmpty().When(x => x.NewStatus == RequestStatus.Resolved)
            .WithMessage("A resolution note is required when resolving a request.");
    }
}

public sealed class AddNoteDtoValidator : AbstractValidator<AddNoteDto>
{
    public AddNoteDtoValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0).WithMessage("Choose a request.");
        RuleFor(x => x.Note).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Note is required.")
            .MaximumLength(RequestRules.MaxNoteLength)
                .WithMessage($"Note must be at most {RequestRules.MaxNoteLength} characters.");
    }
}
