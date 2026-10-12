using FluentValidation;
using FootballSocialMedia.Model.Polls.Dtos.Requests;

namespace FootballSocialMedia.Service.Polls.Validator;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required.")
            .MaximumLength(500).WithMessage("Question cannot exceed 500 characters.");

        RuleFor(x => x.Options)
            .NotNull()
            .Must(o => o != null && o.Count(x => !string.IsNullOrWhiteSpace(x)) >= 2)
            .WithMessage("A poll needs at least 2 options.")
            .Must(o => o == null || o.Count <= 10)
            .WithMessage("A poll can have at most 10 options.");

        RuleForEach(x => x.Options).MaximumLength(200).WithMessage("Option text cannot exceed 200 characters.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(_ => DateTime.UtcNow).WithMessage("Expiry date must be in the future.");
    }
}
