using FluentValidation;
using FootballSocialMedia.Model.Follows.Dtos.Requests;

namespace FootballSocialMedia.Service.Follows.Validator;

public class CreateFollowRequestValidator : AbstractValidator<CreateFollowRequest>
{
    public CreateFollowRequestValidator()
    {
        RuleFor(x => x.FollowerId)
            .NotEmpty().WithMessage("FollowerId is required.");

        RuleFor(x => x.FollowingId)
            .NotEmpty().WithMessage("FollowingId is required.");

        RuleFor(x => x.FollowerId)
            .NotEqual(x => x.FollowingId).WithMessage("You cannot follow yourself.");
    }
}
