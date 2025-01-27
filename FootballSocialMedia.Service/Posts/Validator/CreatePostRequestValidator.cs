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

        RuleFor(request => request.ImageUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("ImageUrl must be a valid URL.");

    }
}
