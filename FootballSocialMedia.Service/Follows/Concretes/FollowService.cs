using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Model.Follows.Dtos.Responses;
using FootballSocialMedia.Model.Follows.Entities;
using FootballSocialMedia.Repository.Follows.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Follows.Abstracts;
using System.Net;

namespace FootballSocialMedia.Service.Follows.Concretes;

public class FollowService(IFollowRepository followRepository, IMapper mapper, IUnitOfWork unitOfWork) : IFollowService
{
    public async Task<ServiceResult<FollowDto>> FollowAsync(CreateFollowRequest request)
    {
        var follow = mapper.Map<Follow>(request);

        var existingFollow = await followRepository.GetByIdAsync(request.FollowerId, request.FollowingId);
        if (existingFollow != null)
        {
            return ServiceResult<FollowDto>.Fail("Already following.", HttpStatusCode.BadRequest);
        }

        await followRepository.AddAsync(follow);
        await unitOfWork.SaveChangesAsync();

        var followResponse = mapper.Map<FollowDto>(follow);
        return ServiceResult<FollowDto>.SuccessAsCreated(followResponse, $"api/follows/{follow.FollowerId}/{follow.FollowingId}");
    }

    public async Task<ServiceResult> UnfollowAsync(string followerId, string followingId)
    {
        var follow = await followRepository.GetByIdAsync(followerId, followingId);
        if (follow == null)
        {
            return ServiceResult.Fail("Follow relationship not found.", HttpStatusCode.NotFound);
        }

        followRepository.Delete(follow);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
    public async Task<ServiceResult<List<FollowDto>>> GetFollowingAsync(string followerId)
    {
        var follows = await followRepository.GetByFollowerIdAsync(followerId);
        var followsDto = mapper.Map<List<FollowDto>>(follows);

        return ServiceResult<List<FollowDto>>.Success(followsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<FollowDto>>> GetFollowersAsync(string followingId)
    {
        var follows = await followRepository.GetByFollowingIdAsync(followingId);
        var followsDto = mapper.Map<List<FollowDto>>(follows);

        return ServiceResult<List<FollowDto>>.Success(followsDto, HttpStatusCode.OK);
    }
}
