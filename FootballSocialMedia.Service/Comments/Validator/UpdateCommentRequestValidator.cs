using FluentValidation;
using FootballSocialMedia.Model.Comments.Dtos.Requests;

namespace FootballSocialMedia.Service.Comments.Validator;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Comment ID cannot be empty.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content cannot be empty.")
            .MinimumLength(1).WithMessage("Comment must be at least 1 character long.")
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
    }
}
