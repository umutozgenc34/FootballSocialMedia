using FluentValidation;
using FootballSocialMedia.Model.Users.Dtos.Requests;

namespace FootballSocialMedia.Service.Users.Validator;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")
            .When(x => !string.IsNullOrEmpty(x.UserName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.FavoriteTeam).MaximumLength(100).WithMessage("Takım adı en fazla 100 karakter olabilir.");
        RuleFor(x => x.Bio).MaximumLength(500).WithMessage("Biyografi en fazla 500 karakter olabilir.");
    }
}
