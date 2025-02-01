using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Comments.Dtos.Requests;
using FootballSocialMedia.Model.Comments.Dtos.Responses;

namespace FootballSocialMedia.Service.Comments.Abstracts;

public interface ICommentService
{
    Task<ServiceResult<CreateCommentResponse>> CreateAsync(CreateCommentRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<ServiceResult<List<CommentDto>>> GetAllAsync();
    Task<ServiceResult<CommentDto>> GetByIdAsync(Guid id);
    Task<ServiceResult> UpdateAsync(UpdateCommentRequest request);
    Task<ServiceResult<List<CommentDto>>> GetCommentsByPostIdAsync(Guid postId);
    Task<ServiceResult<List<CommentDto>>> GetCommentsByUserIdAsync(string userId);
}