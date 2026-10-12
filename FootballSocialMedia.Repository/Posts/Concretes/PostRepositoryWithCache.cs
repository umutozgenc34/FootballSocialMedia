using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Posts.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

namespace FootballSocialMedia.Repository.Posts.Concretes;

/// <summary>
/// IPostRepository için Redis cache decorator'ı (Scrutor ile sarılıyor).
/// - Sadece GetAllAsync cache'lenir (GET api/v1/posts).
/// - GetByIdAsync bilerek cache'lenmez: Update/Delete akışında kullanılıyor ve cache'ten gelen (track edilmeyen,
///   içinde User navigation'ı olan) entity'yi Update etmek kullanıcı satırını da eski veriyle güncelliyordu.
/// - Redis kapalıysa hata fırlatmak yerine doğrudan veritabanına düşer.
/// </summary>
public class PostRepositoryWithCache(IPostRepository innerRepository, IDistributedCache distributedCache,
    ILogger<PostRepositoryWithCache> logger) : IPostRepository
{
    private const string AllPostsKey = "post_all_posts";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<List<Post>> GetAllAsync()
    {
        try
        {
            var cachedData = await distributedCache.GetStringAsync(AllPostsKey);
            if (cachedData != null)
                return JsonSerializer.Deserialize<List<Post>>(cachedData, JsonOptions)!;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis okunamadı, veritabanına düşülüyor.");
            return await innerRepository.GetAllAsync();
        }

        var posts = await innerRepository.GetAllAsync();

        // Şifre hash'i vb. cache'e yazılmasın diye User'ı sadeleştirerek saklıyoruz
        var slim = posts.Select(p => new Post
        {
            Id = p.Id,
            Content = p.Content,
            ImageUrls = p.ImageUrls,
            VideoUrls = p.VideoUrls,
            UserId = p.UserId,
            User = new Model.Users.Entities.User { Id = p.User.Id, UserName = p.User.UserName },
            CreatedTime = p.CreatedTime,
            UpdatedTime = p.UpdatedTime,
            Likes = p.Likes.Select(l => new Model.Likes.Entities.Like { Id = l.Id, UserId = l.UserId, PostId = l.PostId }).ToList(),
            Comments = p.Comments.Select(c => new Model.Comments.Comment { Id = c.Id, PostId = c.PostId, UserId = c.UserId, Content = c.Content }).ToList()
        }).ToList();

        await TryCacheAsync(() => distributedCache.SetStringAsync(AllPostsKey, JsonSerializer.Serialize(slim, JsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) }));

        return posts;
    }

    public ValueTask<Post?> GetByIdAsync(Guid id) => innerRepository.GetByIdAsync(id);

    public IQueryable<Post> Where(Expression<Func<Post, bool>>? predicate = null) => innerRepository.Where(predicate);

    public async ValueTask AddAsync(Post entity)
    {
        await innerRepository.AddAsync(entity);
        await InvalidateAsync();
    }

    public void Update(Post entity)
    {
        innerRepository.Update(entity);
        InvalidateAsync().GetAwaiter().GetResult();
    }

    public void Delete(Post entity)
    {
        innerRepository.Delete(entity);
        InvalidateAsync().GetAwaiter().GetResult();
    }

    public async Task DeleteWithRelationsAsync(Post post)
    {
        await innerRepository.DeleteWithRelationsAsync(post);
        await InvalidateAsync();
    }

    public Task<List<Post>> GetPostsWithCommentsAsync() => innerRepository.GetPostsWithCommentsAsync();

    private Task InvalidateAsync() => TryCacheAsync(() => distributedCache.RemoveAsync(AllPostsKey));

    private async Task TryCacheAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) { logger.LogWarning(ex, "Redis işlemi başarısız, cache atlanıyor."); }
    }
}
