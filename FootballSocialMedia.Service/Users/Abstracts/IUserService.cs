using Core.CrossCuttingConcerns.Response;
using Core.Security.Dtos;
using FootballSocialMedia.Model.Users.Dtos.Requests;
using FootballSocialMedia.Model.Users.Dtos.Responses;

namespace FootballSocialMedia.Service.Users.Abstracts;

public interface IUserService
{
    Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName);
    Task<ServiceResult<List<UserDto>>> GetAllAsync();
    Task<ServiceResult<UserDto>> GetByIdAsync(string id);
    Task<ServiceResult> UpdateAsync(UpdateUserRequest request);
    Task<ServiceResult> DeleteAsync(string id);
}
