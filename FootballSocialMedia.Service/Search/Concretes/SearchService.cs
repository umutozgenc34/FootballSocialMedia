using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Search.Dtos;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.Posts.Abstracts;
using FootballSocialMedia.Service.Search.Abstracts;
using FootballSocialMedia.Service.Users.Abstracts;
using Microsoft.Extensions.Logging;

namespace FootballSocialMedia.Service.Search.Concretes;

/// <summary>
/// Postlar Elasticsearch ile (içerik + yorumlar, fuzzy), kullanıcılar SQL ile aranır.
/// Elasticsearch'e ulaşılamazsa post araması SQL LIKE'a düşer, böylece arama hiç çalışmaz hale gelmez.
/// </summary>
public class SearchService(IElasticsearchService elasticsearchService, IPostService postService, IUserService userService,
    ILogger<SearchService> logger) : ISearchService
{
    private const int MaxResults = 30;

    public async Task<ServiceResult<SearchResultDto>> SearchAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return ServiceResult<SearchResultDto>.Fail("Search text is required.");

        var text = searchText.Trim();
        var users = (await userService.SearchAsync(text, 10)).Data ?? new();

        List<PostDto> posts;
        var engine = "elasticsearch";
        try
        {
            var ids = await elasticsearchService.SearchPostIdsAsync(text, MaxResults);
            posts = await postService.GetByIdsAsync(ids);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch araması başarısız, SQL'e düşülüyor.");
            posts = await postService.SearchInDatabaseAsync(text, MaxResults);
            engine = "sql";
        }

        return ServiceResult<SearchResultDto>.Success(new SearchResultDto { Users = users, Posts = posts, Engine = engine });
    }

    public async Task<ServiceResult<AutoCompleteDto>> AutoCompleteAsync(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return ServiceResult<AutoCompleteDto>.Success(new AutoCompleteDto());

        var users = (await userService.SearchAsync(prefix.Trim(), 5)).Data ?? new();

        List<string> posts;
        try
        {
            posts = await elasticsearchService.AutoCompleteAsync(prefix.Trim());
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Autocomplete için Elasticsearch'e ulaşılamadı.");
            posts = new List<string>();
        }

        return ServiceResult<AutoCompleteDto>.Success(new AutoCompleteDto { Users = users, Posts = posts });
    }
}
