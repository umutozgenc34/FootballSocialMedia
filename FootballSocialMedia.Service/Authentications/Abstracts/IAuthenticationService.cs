using Core.CrossCuttingConcerns.Response;
using Core.Security.Dtos;

namespace FootballSocialMedia.Service.Authentications.Abstracts;

public interface IAuthenticationService
{
    Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto);
    Task<ServiceResult<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
    Task<ServiceResult> RevokeRefreshToken(string refreshToken);
}
