using FluentValidation;
using FootballSocialMedia.Model.Follows.Dtos.Requests;

namespace FootballSocialMedia.Service.Follows.Validator;

public class CreateFollowRequestValidator : AbstractValidator<CreateFollowRequest>
{
    public CreateFollowRequestValidator()
    {
        RuleFor(x => x.FollowingId)
            .NotEmpty().WithMessage("FollowingId is required.");
    }
}
