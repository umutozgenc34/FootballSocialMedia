using FluentValidation;
using FootballSocialMedia.Model.Likes.Dtos.Requests;

namespace FootballSocialMedia.Service.Likes.Validator;

public class CreateLikeRequestValidator : AbstractValidator<CreateLikeRequest>
{
    public CreateLikeRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => (x.PostId is not null) ^ (x.CommentId is not null))
            .WithMessage("Exactly one of PostId or CommentId must be provided.");
    }
}
