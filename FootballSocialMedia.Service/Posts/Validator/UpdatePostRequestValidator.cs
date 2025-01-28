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
            .Must(files => files == null || files.All(f => f.Length > 0))
            .WithMessage("All uploaded images must be valid.")
            .Must(files => files == null || files.Count() <= 4)
            .WithMessage("You can upload a maximum of 4 images.");

        RuleFor(request => request.VideoUrls)
            .Must(files => files == null || files.All(f => f.Length > 0))
            .WithMessage("All uploaded videos must be valid.")
            .Must(files => files == null || files.Count() <= 4)
            .WithMessage("You can upload a maximum of 4 videos.");

    }
}
