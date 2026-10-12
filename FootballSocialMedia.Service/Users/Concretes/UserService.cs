using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Security.Constants;
using Core.Security.Dtos;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Users.Dtos.Requests;
using FootballSocialMedia.Model.Users.Dtos.Responses;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Follows.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Repository.Users.Abstracts;
using FootballSocialMedia.Service.Users.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FootballSocialMedia.Service.Users.Concretes;

public class UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper,
    IUserProfileRepository userProfileRepository, IFollowRepository followRepository, IUnitOfWork unitOfWork,
    ICloudinaryService cloudinaryService, IIdGenerator idGenerator, IIdentityService identityService) : IUserService
{
    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User { Email = registerDto.Email, UserName = registerDto.UserName };

        // Eskiden önce rol ekleniyor, sonra CreateAsync sonucu kontrol ediliyordu: boş DB'de "user" rolü olmadığı için
        // kayıt hata dönüyor ama kullanıcı yine de oluşmuş oluyordu.
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return ServiceResult<UserDto>.Fail(errors, HttpStatusCode.BadRequest);
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.User))
            await roleManager.CreateAsync(new IdentityRole(RoleNames.User));

        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.User);
        if (!roleResult.Succeeded)
            return ServiceResult<UserDto>.Fail(roleResult.Errors.First().Description);

        await userProfileRepository.AddAsync(new UserProfile { Id = idGenerator.GenerateId(), UserId = user.Id });
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(mapper.Map<UserDto>(user), HttpStatusCode.Created);
    }

    public async Task<ServiceResult> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return ServiceResult.Fail(result.Errors.First().Description);

        return ServiceResult.Success("User deleted.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<UserDto>>> GetAllAsync()
    {
        var users = await userManager.Users.ToListAsync();
        return ServiceResult<List<UserDto>>.Success(mapper.Map<List<UserDto>>(users), HttpStatusCode.OK);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user is null
            ? ServiceResult<UserDto>.Fail("User not found", HttpStatusCode.NotFound)
            : ServiceResult<UserDto>.Success(mapper.Map<UserDto>(user), HttpStatusCode.OK);
    }

    public async Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        return user is null
            ? ServiceResult<UserDto>.Fail("UserName not found", HttpStatusCode.NotFound)
            : ServiceResult<UserDto>.Success(mapper.Map<UserDto>(user), HttpStatusCode.OK);
    }

    public Task<ServiceResult<UserProfileResponse>> GetMeAsync() => GetProfileAsync(identityService.GetUserId);

    public async Task<ServiceResult<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return ServiceResult<UserProfileResponse>.Fail("User not found", HttpStatusCode.NotFound);

        var me = identityService.GetUserId;
        var isMe = me == user.Id;

        var response = new UserProfileResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = isMe ? user.Email : null,
            FavoriteTeam = user.Profile?.FavoriteTeam,
            Bio = user.Profile?.Bio,
            ProfileImageUrl = user.Profile?.ProfileImageUrl,
            PostCount = await userManager.Users.Where(u => u.Id == userId).Select(u => u.Posts.Count).FirstAsync(),
            FollowerCount = await followRepository.CountFollowersAsync(userId),
            FollowingCount = await followRepository.CountFollowingAsync(userId),
            IsMe = isMe,
            IsFollowedByMe = !isMe && !string.IsNullOrEmpty(me) && await followRepository.GetByIdAsync(me, userId) is not null,
            Roles = isMe ? (await userManager.GetRolesAsync(user)).ToList() : new List<string>(),
            CreatedTime = user.CreatedTime
        };

        return ServiceResult<UserProfileResponse>.Success(response);
    }

    public async Task<ServiceResult<List<UserSummaryDto>>> SearchAsync(string query, int take = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return ServiceResult<List<UserSummaryDto>>.Success(new List<UserSummaryDto>());

        var q = query.Trim();
        var users = await userManager.Users
            .Where(u => u.UserName!.Contains(q) || (u.Profile != null && u.Profile.FavoriteTeam != null && u.Profile.FavoriteTeam.Contains(q)))
            .OrderBy(u => u.UserName)
            .Take(Math.Clamp(take, 1, 50))
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                ProfileImageUrl = u.Profile != null ? u.Profile.ProfileImageUrl : null,
                FavoriteTeam = u.Profile != null ? u.Profile.FavoriteTeam : null
            })
            .ToListAsync();

        return ServiceResult<List<UserSummaryDto>>.Success(users);
    }

    public async Task<ServiceResult<List<UserSummaryDto>>> GetSuggestionsAsync(int take = 5)
    {
        var me = identityService.GetUserId;
        var following = await followRepository.GetFollowingIdsAsync(me);
        following.Add(me);

        // Takip etmediğim, en çok post atan kullanıcılar
        var users = await userManager.Users
            .Where(u => !following.Contains(u.Id))
            .OrderByDescending(u => u.Posts.Count)
            .ThenByDescending(u => u.CreatedTime)
            .Take(Math.Clamp(take, 1, 20))
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                ProfileImageUrl = u.Profile != null ? u.Profile.ProfileImageUrl : null,
                FavoriteTeam = u.Profile != null ? u.Profile.FavoriteTeam : null
            })
            .ToListAsync();

        return ServiceResult<List<UserSummaryDto>>.Success(users);
    }

    public async Task<ServiceResult<UserProfileResponse>> UpdateAsync(UpdateUserRequest request)
    {
        var user = await userManager.FindByIdAsync(identityService.GetUserId);
        if (user == null)
            return ServiceResult<UserProfileResponse>.Fail("User not found.", HttpStatusCode.NotFound);

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email.Trim();
        if (!string.IsNullOrWhiteSpace(request.UserName))
            user.UserName = request.UserName.Trim();

        var userResult = await userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
            return ServiceResult<UserProfileResponse>.Fail(userResult.Errors.Select(e => e.Description).ToList());

        var userProfile = await userProfileRepository.GetByUserIdAsync(user.Id);
        if (userProfile == null)
        {
            userProfile = new UserProfile { Id = idGenerator.GenerateId(), UserId = user.Id };
            await userProfileRepository.AddAsync(userProfile);
        }

        userProfile.FavoriteTeam = string.IsNullOrWhiteSpace(request.FavoriteTeam) ? null : request.FavoriteTeam.Trim();
        userProfile.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();

        if (request.ProfileImage != null)
        {
            var uploadUrls = await cloudinaryService.UploadImages(new List<IFormFile> { request.ProfileImage }, "profile_images");
            if (uploadUrls.Any())
                userProfile.ProfileImageUrl = uploadUrls.First();
        }

        await unitOfWork.SaveChangesAsync();

        return await GetProfileAsync(user.Id);
    }
}
