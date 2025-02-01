using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;

namespace FootballSocialMedia.Service.Posts.Abstracts;

public interface IPostService
{
    Task<ServiceResult<List<PostDto>>> GetAllAsync();
    Task<ServiceResult<PostDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<CreatePostResponse>> CreateAsync(CreatePostRequest request);
    Task<ServiceResult> UpdateAsync(UpdatePostRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<ServiceResult<List<PostWithCommentsResponse>>> GetPostsWithCommentsAsync();
}
