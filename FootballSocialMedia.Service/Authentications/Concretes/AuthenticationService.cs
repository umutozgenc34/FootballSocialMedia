using Core.CrossCuttingConcerns.Response;
using Core.Security.Dtos;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Authentications.Abstracts;
using FootballSocialMedia.Service.JWT.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FootballSocialMedia.Service.Authentications.Concretes;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRefreshTokenService _userRefreshTokenService;
    public AuthenticationService(ITokenService tokenService, UserManager<User> userManager, IUnitOfWork unitOfWork, IUserRefreshTokenService userRefreshTokenService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userRefreshTokenService = userRefreshTokenService;
    }
    public async Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
        if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
        var user = await _userManager.FindByEmailAsync(loginDto.Email)
                   ?? await _userManager.FindByNameAsync(loginDto.Email);
        if (user == null) return ServiceResult<TokenDto>.Fail("Email or Password wrong.", HttpStatusCode.BadRequest);
        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return ServiceResult<TokenDto>.Fail("Email or Password wrong.", HttpStatusCode.BadRequest);
        }
        var token = await _tokenService.CreateTokenAsync(user);
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.Id == user.Id).SingleOrDefaultAsync();
        if (userRefreshToken == null)
        {
            await _userRefreshTokenService.AddAsync(new UserRefreshToken { Id = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
        }
        else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<TokenDto>.Success(token, HttpStatusCode.Created);
    }
    public async Task<ServiceResult<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
        if (existRefreshToken == null)
        {
            return ServiceResult<TokenDto>.Fail("Refresh token not found.", HttpStatusCode.NotFound);
        }
        var user = await _userManager.FindByIdAsync(existRefreshToken.Id);
        if (user == null)
        {
            return ServiceResult<TokenDto>.Fail("User Id not found.", HttpStatusCode.NotFound);
        }
        var tokenDto = await _tokenService.CreateTokenAsync(user);
        existRefreshToken.Code = tokenDto.RefreshToken;
        existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<TokenDto>.Success(tokenDto, HttpStatusCode.Created);
    }
    public async Task<ServiceResult> RevokeRefreshToken(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
        if (existRefreshToken == null)
        {
            return ServiceResult.Fail("Refresh token not found.", HttpStatusCode.NotFound);
        }
        _userRefreshTokenService.Delete(existRefreshToken);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success("Refresh token silindi.", HttpStatusCode.NoContent);
    }
}