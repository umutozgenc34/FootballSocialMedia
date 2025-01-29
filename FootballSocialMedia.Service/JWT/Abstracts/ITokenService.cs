using Core.Security.Dtos;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Service.JWT.Abstracts;

public interface ITokenService
{
    Task<TokenDto> CreateTokenAsync(User user);
}
