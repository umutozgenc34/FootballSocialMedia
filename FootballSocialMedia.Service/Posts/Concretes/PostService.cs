using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Security.Constants;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Search.Dtos;
using FootballSocialMedia.Repository.Follows.Abstracts;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.Posts.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;

namespace FootballSocialMedia.Service.Posts.Concretes;

public class PostService(IPostRepository postRepository, IMapper mapper, IUnitOfWork unitOfWork,
    IIdGenerator idGenerator, ICloudinaryService cloudinaryService, IElasticsearchService elasticsearchService,
    IIdentityService identityService, IFollowRepository followRepository) : IPostService
{
    private const int MaxPageSize = 50;

    /// <summary>
    /// Post'u, beğeni/yorum sayıları ve "ben beğendim mi" bilgisiyle birlikte tek SQL sorgusunda DTO'ya çevirir.
    /// </summary>
    private static Expression<Func<Post, PostDto>> ToDto(string currentUserId) => p => new PostDto
    {
        Id = p.Id,
        Content = p.Content,
        ImageUrls = p.ImageUrls!,
        VideoUrls = p.VideoUrls,
        UserId = p.UserId,
        Username = p.User.UserName!,
        ProfileImageUrl = p.User.Profile != null ? p.User.Profile.ProfileImageUrl : null,
        FavoriteTeam = p.User.Profile != null ? p.User.Profile.FavoriteTeam : null,
        LikeCount = p.Likes.Count,
        CommentCount = p.Comments.Count,
        IsLikedByMe = p.Likes.Any(l => l.UserId == currentUserId),
        CreatedTime = p.CreatedTime,
        UpdatedTime = p.UpdatedTime
    };

    private static PostDto Normalize(PostDto dto) => dto with
    {
        ImageUrls = dto.ImageUrls ?? new List<string>(),
        VideoUrls = dto.VideoUrls ?? new List<string>()
    };

    private async Task<List<PostDto>> QueryAsync(IQueryable<Post> query, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var posts = await query
            .OrderByDescending(p => p.CreatedTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();

        return posts.Select(Normalize).ToList();
    }

    public async Task<ServiceResult<List<PostDto>>> GetFeedAsync(string mode, int page, int pageSize)
    {
        var query = postRepository.Where();

        if (string.Equals(mode, "following", StringComparison.OrdinalIgnoreCase))
        {
            var me = identityService.GetUserId;
            var ids = await followRepository.GetFollowingIdsAsync(me);
            ids.Add(me);
            query = query.Where(p => ids.Contains(p.UserId));
        }

        return ServiceResult<List<PostDto>>.Success(await QueryAsync(query, page, pageSize));
    }

    public async Task<ServiceResult<List<PostDto>>> GetUserPostsAsync(string userId, int page, int pageSize)
    {
        var posts = await QueryAsync(postRepository.Where(p => p.UserId == userId), page, pageSize);
        return ServiceResult<List<PostDto>>.Success(posts);
    }

    public async Task<ServiceResult<List<PostDto>>> GetAllAsync()
    {
        // Redis cache'li liste (PostRepositoryWithCache)
        var posts = await postRepository.GetAllAsync();
        var postsAsDto = mapper.Map<List<PostDto>>(posts);
        return ServiceResult<List<PostDto>>.Success(postsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<PostDto>> GetByIdAsync(Guid id)
    {
        var post = await postRepository.Where(p => p.Id == id)
            .Select(ToDto(identityService.GetUserId))
            .FirstOrDefaultAsync();

        return post is null
            ? ServiceResult<PostDto>.Fail("Post not found", HttpStatusCode.NotFound)
            : ServiceResult<PostDto>.Success(Normalize(post), HttpStatusCode.OK);
    }

    public async Task<List<PostDto>> GetByIdsAsync(IReadOnlyList<Guid> ids)
    {
        if (ids.Count == 0) return new List<PostDto>();

        var posts = await postRepository.Where(p => ids.Contains(p.Id))
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();

        var order = ids.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);
        return posts.OrderBy(p => order[p.Id]).Select(Normalize).ToList();
    }

    public async Task<List<PostDto>> SearchInDatabaseAsync(string searchText, int size)
    {
        // LIKE: SQL Server'da da in-memory DB'de de büyük/küçük harf duyarsız çalışır
        var pattern = $"%{EscapeLike(searchText)}%";
        var posts = await postRepository
            .Where(p => EF.Functions.Like(p.Content, pattern) || p.Comments.Any(c => EF.Functions.Like(c.Content, pattern)))
            .OrderByDescending(p => p.CreatedTime)
            .Take(size)
            .Select(ToDto(identityService.GetUserId))
            .ToListAsync();

        return posts.Select(Normalize).ToList();
    }

    public async Task<ServiceResult<PostDto>> CreateAsync(CreatePostRequest request)
    {
        var imageUrls = new List<string>();
        var videoUrls = new List<string>();

        if (request.ImageUrls != null && request.ImageUrls.Any())
            imageUrls = await cloudinaryService.UploadImages(request.ImageUrls, "post-images");

        if (request.VideoUrls != null && request.VideoUrls.Any())
            videoUrls = await cloudinaryService.UploadVideos(request.VideoUrls, "post-videos");

        var post = new Post
        {
            Id = idGenerator.GenerateId(),
            Content = request.Content,
            UserId = identityService.GetUserId,
            ImageUrls = imageUrls,
            VideoUrls = videoUrls
        };

        await postRepository.AddAsync(post);
        await unitOfWork.SaveChangesAsync();

        // Önce DB'ye yaz, sonra index'le: ES kapalıysa post yine de oluşsun.
        await elasticsearchService.IndexPostAsync(new PostSearchDocument
        {
            Id = post.Id,
            Content = post.Content,
            UserId = post.UserId,
            UserName = identityService.UserName,
            CreatedTime = post.CreatedTime
        });

        var created = (await GetByIdAsync(post.Id)).Data ?? new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            UserId = post.UserId,
            Username = identityService.UserName,
            ImageUrls = imageUrls,
            VideoUrls = videoUrls,
            CreatedTime = post.CreatedTime
        };

        return ServiceResult<PostDto>.SuccessAsCreated(created, $"api/v1/posts/{post.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(UpdatePostRequest request)
    {
        var post = await postRepository.GetByIdAsync(request.Id);
        if (post is null)
            return ServiceResult.Fail("Post not found.", HttpStatusCode.NotFound);

        if (post.UserId != identityService.GetUserId)
            return ServiceResult.Fail("You can only edit your own posts.", HttpStatusCode.Forbidden);

        if (request.ImageUrls != null && request.ImageUrls.Any())
            post.ImageUrls = await cloudinaryService.UploadImages(request.ImageUrls, "post-images");

        if (request.VideoUrls != null && request.VideoUrls.Any())
            post.VideoUrls = await cloudinaryService.UploadVideos(request.VideoUrls, "post-videos");

        post.Content = request.Content;

        postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        await elasticsearchService.IndexPostAsync(new PostSearchDocument
        {
            Id = post.Id,
            Content = post.Content,
            UserId = post.UserId,
            UserName = post.User?.UserName,
            CreatedTime = post.CreatedTime
        });

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null)
            return ServiceResult.Fail("Post not found", HttpStatusCode.NotFound);

        if (post.UserId != identityService.GetUserId && !identityService.IsInRole(RoleNames.Admin))
            return ServiceResult.Fail("You can only delete your own posts.", HttpStatusCode.Forbidden);

        await postRepository.DeleteWithRelationsAsync(post);
        await unitOfWork.SaveChangesAsync();
        await elasticsearchService.DeletePostAsync(id);

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<PostWithCommentsResponse>>> GetPostsWithCommentsAsync()
    {
        var posts = await postRepository.GetPostsWithCommentsAsync();
        var postsAsDto = mapper.Map<List<PostWithCommentsResponse>>(posts);
        return ServiceResult<List<PostWithCommentsResponse>>.Success(postsAsDto, HttpStatusCode.OK);
    }

    private static string EscapeLike(string text) =>
        text.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
}
