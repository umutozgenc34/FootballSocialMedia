using Core.Security.Dtos;
using FluentValidation;

namespace FootballSocialMedia.Service.Users.Validator;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("E-posta veya kullanıcı adı boş olamaz."); // e-posta ya da kullanıcı adıyla giriş yapılabilir
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.");
    }
}
