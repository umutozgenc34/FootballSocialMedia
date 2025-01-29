using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Security.Dtos;
using FootballSocialMedia.Model.Users.Dtos.Requests;
using FootballSocialMedia.Model.Users.Dtos.Responses;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Service.Users.Abstracts;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace FootballSocialMedia.Service.Users.Concretes;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    public UserService(UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;

    }
    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User { Email = registerDto.Email, UserName = registerDto.UserName };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        var role = await _userManager.AddToRoleAsync(user, "user");
        if (!role.Succeeded)
        {
            return ServiceResult<UserDto>.Fail(role.Errors.First().Description);
        }
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return ServiceResult<UserDto>.Fail(errors, HttpStatusCode.BadRequest);
        }
        var userAsDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.Created);
    }
   
    public async Task<ServiceResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.Errors.First().Description);
        }
        return ServiceResult.Success("User deleted.", HttpStatusCode.NoContent);
    }
    public async Task<ServiceResult<List<UserDto>>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        if (!users.Any())
        {
            return ServiceResult<List<UserDto>>.Fail("Users not found", HttpStatusCode.NotFound);
        }
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return ServiceResult<List<UserDto>>.Success(userDtos, HttpStatusCode.OK);
    }
    public async Task<ServiceResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<UserDto>.Fail("User not found", HttpStatusCode.NotFound);
        }
        var userDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userDto, HttpStatusCode.OK);
    }
    public async Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("UserName not found", HttpStatusCode.NotFound);
        }
        var userAsDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.OK);
    }
    public async Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return ServiceResult.Fail("User not found", HttpStatusCode.NotFound);
        }
        user.UserName = request.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded is false)
        {
            return ServiceResult.Fail(result.Errors.First().Description);
        }
        return ServiceResult.Success("User updated.", HttpStatusCode.NoContent);
    }
}