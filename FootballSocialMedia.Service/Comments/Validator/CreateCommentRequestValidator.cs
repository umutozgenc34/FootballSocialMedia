using FluentValidation;
using FootballSocialMedia.Model.Comments.Dtos.Requests;

namespace FootballSocialMedia.Service.Comments.Validator;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content cannot be empty.")
            .MinimumLength(1).WithMessage("Comment must be at least 1 character long.")
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");

        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID cannot be empty.");
    }
}
