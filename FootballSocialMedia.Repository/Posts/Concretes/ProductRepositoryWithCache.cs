using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Posts.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

public class PostRepositoryWithCache : IPostRepository
{
    private readonly IPostRepository _innerRepository;  
    private readonly IDistributedCache _distributedCache;  
    private const string CacheKeyPrefix = "post_";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve, 
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };
    public PostRepositoryWithCache(IPostRepository innerRepository, IDistributedCache distributedCache)
    {
        _innerRepository = innerRepository;
        _distributedCache = distributedCache;
    }

    public async Task<List<Post>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all_posts";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            var cachedPosts = JsonSerializer.Deserialize<List<Post>>(cachedData, _jsonSerializerOptions);
            return cachedPosts!;
        }

        var posts = await _innerRepository.GetAllAsync();


        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(posts, _jsonSerializerOptions), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return posts;
    }

    public async ValueTask<Post?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<Post>(cachedData, _jsonSerializerOptions);
        }

        var post = await _innerRepository.GetByIdAsync(id);

        if (post != null)
        {
            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(post, _jsonSerializerOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return post;
    }

    public IQueryable<Post> Where(Expression<Func<Post, bool>>? predicate = null)
    {
        return _innerRepository.Where(predicate);
    }

    public async ValueTask AddAsync(Post entity)
    {
        await _innerRepository.AddAsync(entity);
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        await _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_posts");
    }

    public void Update(Post entity)
    {
        _innerRepository.Update(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_posts");
    }

    public void Delete(Post entity)
    {
        _innerRepository.Delete(entity);
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}{entity.Id}");
        _distributedCache.RemoveAsync($"{CacheKeyPrefix}all_posts");
    }

    public Task<List<Post>> GetPostsWithCommentsAsync()
    {
       return _innerRepository.GetPostsWithCommentsAsync();
    }
}
