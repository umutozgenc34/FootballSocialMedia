using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Comments.Dtos.Requests;
using FootballSocialMedia.Model.Comments.Dtos.Responses;
using FootballSocialMedia.Repository.Comments.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Comments.Abstracts;
using System.Net;

namespace FootballSocialMedia.Service.Comments.Concretes;

public class CommentService(ICommentRepository commentRepository, IMapper mapper, IUnitOfWork unitOfWork,IIdGenerator idGenerator) : ICommentService
{
    public async Task<ServiceResult<CreateCommentResponse>> CreateAsync(CreateCommentRequest request)
    {
        var comment = mapper.Map<Comment>(request);
        comment.Id = idGenerator.GenerateId();

        await commentRepository.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();

        var commentAsDto = mapper.Map<CreateCommentResponse>(comment);
        return ServiceResult<CreateCommentResponse>.SuccessAsCreated(commentAsDto, $"api/comments/{comment.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment is null)
        {
            return ServiceResult.Fail("Comment not found", HttpStatusCode.NotFound);
        }

        commentRepository.Delete(comment);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetAllAsync()
    {
        var comments = await commentRepository.GetAllAsync();
        var commentsAsDto = mapper.Map<List<CommentDto>>(comments);

        return ServiceResult<List<CommentDto>>.Success(commentsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<CommentDto>> GetByIdAsync(Guid id)
    {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment is null)
        {
            return ServiceResult<CommentDto>.Fail("Comment not found", HttpStatusCode.NotFound);
        }

        var commentAsDto = mapper.Map<CommentDto>(comment);
        return ServiceResult<CommentDto>.Success(commentAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCommentRequest request)
    {
        var comment = await commentRepository.GetByIdAsync(request.Id);
        if (comment is null)
        {
            return ServiceResult.Fail("Comment not found.", HttpStatusCode.NotFound);
        }

        comment.Content = request.Content;

        commentRepository.Update(comment);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetCommentsByPostIdAsync(Guid postId)
    {
        var comments = await commentRepository.GetCommentsByPostIdAsync(postId);
        var commentsAsDto = mapper.Map<List<CommentDto>>(comments);

        return ServiceResult<List<CommentDto>>.Success(commentsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetCommentsByUserIdAsync(string userId)
    {
        var comments = await commentRepository.GetCommentsByUserIdAsync(userId);
        var commentsAsDto = mapper.Map<List<CommentDto>>(comments);

        return ServiceResult<List<CommentDto>>.Success(commentsAsDto, HttpStatusCode.OK);
    }
}
