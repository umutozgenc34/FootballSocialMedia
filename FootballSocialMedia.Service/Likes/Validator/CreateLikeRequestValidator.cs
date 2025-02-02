using FluentValidation;
using FootballSocialMedia.Model.Likes.Dtos.Requests;

namespace FootballSocialMedia.Service.Likes.Validator;

public class CreateLikeRequestValidator : AbstractValidator<CreateLikeRequest>
{
    public CreateLikeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty.");

        RuleFor(x => x.PostId)
            .NotEmpty().When(x => x.CommentId == null)
            .WithMessage("Either PostId or CommentId must be provided.");

        RuleFor(x => x.CommentId)
            .NotEmpty().When(x => x.PostId == null)
            .WithMessage("Either PostId or CommentId must be provided.");
    }
}
