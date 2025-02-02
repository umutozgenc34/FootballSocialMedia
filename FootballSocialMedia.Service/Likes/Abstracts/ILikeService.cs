using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Model.Likes.Dtos.Responses;

namespace FootballSocialMedia.Service.Likes.Abstracts;

public interface ILikeService
{
    Task<ServiceResult<LikeDto>> LikeAsync(CreateLikeRequest request);
    Task<ServiceResult> UnlikeAsync(string userId, Guid? postId = null, Guid? commentId = null);
    Task<ServiceResult<int>> GetLikeCountAsync(Guid? postId = null, Guid? commentId = null);
    Task<ServiceResult<List<LikeDto>>> GetUserLikesAsync(string userId);
}
