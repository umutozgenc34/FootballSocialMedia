using Core.Security.Dtos;
using FluentValidation;

namespace FootballSocialMedia.Service.Users.Validator;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
               .EmailAddress().WithMessage("Geçersiz e-posta adresi.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.");
    }
}
