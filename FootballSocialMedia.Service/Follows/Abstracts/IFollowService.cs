using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Model.Follows.Dtos.Responses;

namespace FootballSocialMedia.Service.Follows.Abstracts;

public interface IFollowService
{
    Task<ServiceResult<FollowDto>> FollowAsync(CreateFollowRequest request);
    Task<ServiceResult> UnfollowAsync(string followingId);
    Task<ServiceResult<List<FollowDto>>> GetFollowingAsync(string followerId);
    Task<ServiceResult<List<FollowDto>>> GetFollowersAsync(string followingId);
}
