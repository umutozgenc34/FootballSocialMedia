using FluentValidation;
using FootballSocialMedia.Model.News.Dtos.Requests;

namespace FootballSocialMedia.Service.Newss.Validator;

public class CreateNewsRequestValidator : AbstractValidator<CreateNewsRequest>
{
    public CreateNewsRequestValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required.")
             .MaximumLength(200).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(5000).WithMessage("Content must be maximum 5000 characters long.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required.")
            .MaximumLength(255).WithMessage("Source cannot exceed 200 characters.");

        RuleFor(x => x.ImageUrl)
            .NotNull().WithMessage("Image is required.");
    }

}
