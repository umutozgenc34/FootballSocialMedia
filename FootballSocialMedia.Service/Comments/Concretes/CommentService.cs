using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Security.Constants;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Comments.Dtos.Requests;
using FootballSocialMedia.Model.Comments.Dtos.Responses;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Model.Notifications.Enums;
using FootballSocialMedia.Model.Search.Dtos;
using FootballSocialMedia.Repository.Comments.Abstracts;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Comments.Abstracts;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;

namespace FootballSocialMedia.Service.Comments.Concretes;

public class CommentService(ICommentRepository commentRepository, IPostRepository postRepository,
    IUnitOfWork unitOfWork, IIdGenerator idGenerator, IElasticsearchService elasticsearchService,
    IIdentityService identityService, INotificationPublisher notificationPublisher) : ICommentService
{
    private static Expression<Func<Comment, CommentDto>> ToDto(string currentUserId) => c => new CommentDto
    {
        Id = c.Id,
        PostId = c.PostId,
        Content = c.Content,
        UserId = c.UserId,
        Username = c.User.UserName!,
        ProfileImageUrl = c.User.Profile != null ? c.User.Profile.ProfileImageUrl : null,
        PostContent = c.Post.Content,
        LikeCount = c.Likes.Count,
        IsLikedByMe = c.Likes.Any(l => l.UserId == currentUserId),
        CreatedTime = c.CreatedTime,
        UpdatedTime = c.UpdatedTime
    };

    private Task<CommentDto?> GetDtoAsync(Guid id) => commentRepository.Where(c => c.Id == id)
        .Select(ToDto(identityService.GetUserId))
        .FirstOrDefaultAsync();

    public async Task<ServiceResult<CommentDto>> CreateAsync(CreateCommentRequest request)
    {
        var post = await postRepository.GetByIdAsync(request.PostId);
        if (post is null)
            return ServiceResult<CommentDto>.Fail("Post not found.", HttpStatusCode.NotFound);

        var comment = new Comment
        {
            Id = idGenerator.GenerateId(),
            Content = request.Content,
            PostId = request.PostId,
            UserId = identityService.GetUserId
        };

        await commentRepository.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();

        await elasticsearchService.IndexCommentAsync(new CommentSearchDocument
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            UserId = comment.UserId,
            UserName = identityService.UserName,
            CreatedTime = comment.CreatedTime
        });

        // Post sahibine bildirim (kendi postuna yorum yapınca değil)
        if (post.UserId != comment.UserId)
        {
            notificationPublisher.SendNotification(new Notification
            {
                UserId = post.UserId,
                SenderId = comment.UserId,
                SenderUserName = identityService.UserName,
                Type = NotificationType.Comment,
                Content = $"{identityService.UserName} commented on your post.",
                PostId = post.Id.ToString(),
                CommentId = comment.Id.ToString()
            });
        }

        var dto = await GetDtoAsync(comment.Id);
        return ServiceResult<CommentDto>.SuccessAsCreated(dto!, $"api/v1/comments/{comment.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment is null)
            return ServiceResult.Fail("Comment not found", HttpStatusCode.NotFound);

        // Yorumu; yorumun sahibi, post'un sahibi veya admin silebilir
        var me = identityService.GetUserId;
        var post = await postRepository.GetByIdAsync(comment.PostId);
        if (comment.UserId != me && post?.UserId != me && !identityService.IsInRole(RoleNames.Admin))
            return ServiceResult.Fail("You are not allowed to delete this comment.", HttpStatusCode.Forbidden);

        await commentRepository.DeleteWithLikesAsync(comment);
        await unitOfWork.SaveChangesAsync();
        await elasticsearchService.DeleteCommentAsync(id);

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetAllAsync()
    {
        var comments = await commentRepository.Where()
            .OrderByDescending(c => c.CreatedTime)
            .Take(200)
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();
        return ServiceResult<List<CommentDto>>.Success(comments, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<CommentDto>> GetByIdAsync(Guid id)
    {
        var comment = await GetDtoAsync(id);
        return comment is null
            ? ServiceResult<CommentDto>.Fail("Comment not found", HttpStatusCode.NotFound)
            : ServiceResult<CommentDto>.Success(comment, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateCommentRequest request)
    {
        var comment = await commentRepository.GetByIdAsync(request.Id);
        if (comment is null)
            return ServiceResult.Fail("Comment not found.", HttpStatusCode.NotFound);

        if (comment.UserId != identityService.GetUserId)
            return ServiceResult.Fail("You can only edit your own comments.", HttpStatusCode.Forbidden);

        comment.Content = request.Content;
        commentRepository.Update(comment);
        await unitOfWork.SaveChangesAsync();

        await elasticsearchService.IndexCommentAsync(new CommentSearchDocument
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            UserId = comment.UserId,
            UserName = identityService.UserName,
            CreatedTime = comment.CreatedTime
        });

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetCommentsByPostIdAsync(Guid postId)
    {
        // Post altında eskiden yeniye sıralı göstermek daha doğal
        var comments = await commentRepository.Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedTime)
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();
        return ServiceResult<List<CommentDto>>.Success(comments, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<CommentDto>>> GetCommentsByUserIdAsync(string userId)
    {
        var comments = await commentRepository.Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedTime)
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();
        return ServiceResult<List<CommentDto>>.Success(comments, HttpStatusCode.OK);
    }
}
