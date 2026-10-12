using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Model.Likes.Dtos.Responses;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Model.Notifications.Enums;
using FootballSocialMedia.Repository.Comments.Abstracts;
using FootballSocialMedia.Repository.Likes.Abstracts;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Likes.Abstracts;
using FootballSocialMedia.Service.RabbitMQ;
using System.Net;

namespace FootballSocialMedia.Service.Likes.Concretes;

public class LikeService(ILikeRepository likeRepository, IPostRepository postRepository, ICommentRepository commentRepository,
    IMapper mapper, IUnitOfWork unitOfWork, IIdGenerator idGenerator, IIdentityService identityService,
    INotificationPublisher notificationPublisher) : ILikeService
{
    public async Task<ServiceResult<LikeDto>> LikeAsync(CreateLikeRequest request)
    {
        var me = identityService.GetUserId;

        // Beğenilen içeriğin sahibini bul (bildirim doğru kişiye gitsin)
        string? ownerId;
        if (request.PostId is not null)
        {
            var post = await postRepository.GetByIdAsync(request.PostId.Value);
            if (post is null) return ServiceResult<LikeDto>.Fail("Post not found.", HttpStatusCode.NotFound);
            ownerId = post.UserId;
        }
        else
        {
            var comment = await commentRepository.GetByIdAsync(request.CommentId!.Value);
            if (comment is null) return ServiceResult<LikeDto>.Fail("Comment not found.", HttpStatusCode.NotFound);
            ownerId = comment.UserId;
        }

        var existingLike = await likeRepository.GetUserLikeAsync(me, request.PostId, request.CommentId);
        if (existingLike is not null)
            return ServiceResult<LikeDto>.Fail("Already liked.", HttpStatusCode.BadRequest);

        var like = new Like
        {
            Id = idGenerator.GenerateId(),
            UserId = me,
            PostId = request.PostId,
            CommentId = request.CommentId
        };

        await likeRepository.AddLikeAsync(like);
        await unitOfWork.SaveChangesAsync();

        // Eskiden bildirim UserId = beğenen kişi olarak gidiyordu, yani herkes kendine bildirim alıyordu.
        if (ownerId != me)
        {
            var isPost = request.PostId is not null;
            notificationPublisher.SendNotification(new Notification
            {
                UserId = ownerId,
                SenderId = me,
                SenderUserName = identityService.UserName,
                Type = NotificationType.Like,
                Content = isPost
                    ? $"{identityService.UserName} liked your post."
                    : $"{identityService.UserName} liked your comment.",
                PostId = request.PostId?.ToString(),
                CommentId = request.CommentId?.ToString()
            });
        }

        var response = new LikeDto
        {
            Id = like.Id,
            UserId = me,
            Username = identityService.UserName,
            PostId = like.PostId,
            CommentId = like.CommentId,
            LikeCount = await likeRepository.GetLikeCountAsync(request.PostId, request.CommentId)
        };

        return ServiceResult<LikeDto>.Success(response, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> UnlikeAsync(Guid? postId = null, Guid? commentId = null)
    {
        if (postId is null && commentId is null)
            return ServiceResult.Fail("Either postId or commentId must be provided.");

        var like = await likeRepository.GetUserLikeAsync(identityService.GetUserId, postId, commentId);
        if (like is null)
            return ServiceResult.Fail("Like not found.", HttpStatusCode.NotFound);

        likeRepository.Delete(like);
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
