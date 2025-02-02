namespace FootballSocialMedia.Service.Likes.Concretes;

using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Model.Likes.Dtos.Responses;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Repository.Likes.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Likes.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;


public class LikeService(ILikeRepository likeRepository, IMapper mapper, IUnitOfWork unitOfWork,IIdGenerator idGenerator) : ILikeService
{
    public async Task<ServiceResult<LikeDto>> LikeAsync(CreateLikeRequest request)
    {
        var existingLike = await likeRepository.Where(l =>
            l.UserId == request.UserId &&
            (l.PostId == request.PostId || l.CommentId == request.CommentId))
            .FirstOrDefaultAsync();

        if (existingLike is not null)
            return ServiceResult<LikeDto>.Fail("Already liked.", HttpStatusCode.BadRequest);

        var like = mapper.Map<Like>(request);
        like.Id=idGenerator.GenerateId();
        await likeRepository.AddLikeAsync(like);
        await unitOfWork.SaveChangesAsync();

        var response = mapper.Map<LikeDto>(like);
        return ServiceResult<LikeDto>.Success(response, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> UnlikeAsync(string userId, Guid? postId = null, Guid? commentId = null)
    {
        var like = await likeRepository.Where(l =>
            l.UserId == userId &&
            (l.PostId == postId || l.CommentId == commentId))
            .FirstOrDefaultAsync();

        if (like is null)
            return ServiceResult.Fail("Like not found.", HttpStatusCode.NotFound);

        await likeRepository.RemoveLikeAsync(userId, postId, commentId);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<int>> GetLikeCountAsync(Guid? postId = null, Guid? commentId = null)
    {
        var likeCount = await likeRepository.GetLikeCountAsync(postId, commentId);
        return ServiceResult<int>.Success(likeCount, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<LikeDto>>> GetUserLikesAsync(string userId)
    {
        var likes = await likeRepository.GetUserLikesAsync(userId);
        var likesAsDto = mapper.Map<List<LikeDto>>(likes);

        return ServiceResult<List<LikeDto>>.Success(likesAsDto, HttpStatusCode.OK);
    }
}
