using Core.Security.Dtos;
using FluentValidation;

namespace FootballSocialMedia.Service.Users.Validator;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")
                .Must(n => n == null || (n == n.Trim() && !n.Contains("  ")))
                .WithMessage("Kullanıcı adı boşlukla başlayıp bitemez ve art arda boşluk içeremez.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .Length(6, 100).WithMessage("Şifre 6 ile 100 karakter arasında olmalıdır.");
    }
}
