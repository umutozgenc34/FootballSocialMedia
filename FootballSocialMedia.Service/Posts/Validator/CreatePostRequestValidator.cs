using FluentValidation;
using FootballSocialMedia.Model.Posts.Dtos.Requests;

namespace FootballSocialMedia.Service.Posts.Validator;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {

        RuleFor(request => request.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");

        RuleFor(request => request.ImageUrls)
         .Must(file => file == null || file.Any(f => f.Length > 0)) 
         .WithMessage("At least one image must be uploaded.");

    }
}
