using FluentValidation;
using FootballSocialMedia.Model.Posts.Dtos.Requests;

namespace FootballSocialMedia.Service.Posts.Validator;

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        RuleFor(x => x.Content)
           
            .MaximumLength(500)
            .WithMessage("Content cannot exceed 500 characters.");

        RuleFor(request => request.ImageUrls)
        .Must(file => file == null || file.Any(f => f.Length > 0)) 
        .WithMessage("At least one image must be uploaded.");

    }
}
