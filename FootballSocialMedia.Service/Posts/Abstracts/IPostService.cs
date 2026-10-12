using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;

namespace FootballSocialMedia.Service.Posts.Abstracts;

public interface IPostService
{
    Task<ServiceResult<List<PostDto>>> GetAllAsync();

    /// <summary>mode: "all" (herkesin postları) veya "following" (takip ettiklerim + benim)</summary>
    Task<ServiceResult<List<PostDto>>> GetFeedAsync(string mode, int page, int pageSize);
    Task<ServiceResult<List<PostDto>>> GetUserPostsAsync(string userId, int page, int pageSize);
    Task<ServiceResult<PostDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<PostDto>> CreateAsync(CreatePostRequest request);
    Task<ServiceResult> UpdateAsync(UpdatePostRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<ServiceResult<List<PostWithCommentsResponse>>> GetPostsWithCommentsAsync();

    /// <summary>Verilen id sırasını koruyarak postları döner (arama sonuçları için).</summary>
    Task<List<PostDto>> GetByIdsAsync(IReadOnlyList<Guid> ids);

    /// <summary>Elasticsearch kapalıyken SQL üzerinden basit içerik araması.</summary>
    Task<List<PostDto>> SearchInDatabaseAsync(string searchText, int size);
}
