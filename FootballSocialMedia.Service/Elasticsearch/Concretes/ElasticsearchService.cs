using Core.Infrastructure.Elasticsearch;
using FootballSocialMedia.Model.Search.Dtos;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using Microsoft.Extensions.Logging;
using Nest;

namespace FootballSocialMedia.Service.Elasticsearch.Concretes;

public class ElasticsearchService(ElasticsearchClient elasticsearchClient, ILogger<ElasticsearchService> logger) : IElasticsearchService
{
    // Post ve yorumlar ayrı index'lerde tutuluyor; eskiden ikisi de aynı index'e yazılıyordu
    // ve SearchAsync<Post> yorumları da post gibi döndürüyordu.
    public const string PostIndex = "fsm-posts";
    public const string CommentIndex = "fsm-comments";

    private IElasticClient Client => elasticsearchClient.GetClient();

    public async Task IndexPostAsync(PostSearchDocument post)
    {
        try
        {
            var response = await Client.IndexAsync(post, i => i.Index(PostIndex).Id(post.Id));
            if (!response.IsValid) logger.LogWarning("Post index'lenemedi: {Info}", response.DebugInformation);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch'e ulaşılamadı, post index'lenmedi.");
        }
    }

    public async Task IndexCommentAsync(CommentSearchDocument comment)
    {
        try
        {
            var response = await Client.IndexAsync(comment, i => i.Index(CommentIndex).Id(comment.Id));
            if (!response.IsValid) logger.LogWarning("Yorum index'lenemedi: {Info}", response.DebugInformation);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch'e ulaşılamadı, yorum index'lenmedi.");
        }
    }

    public async Task DeletePostAsync(Guid postId)
    {
        try
        {
            await Client.DeleteAsync<PostSearchDocument>(postId, d => d.Index(PostIndex));
            await Client.DeleteByQueryAsync<CommentSearchDocument>(d => d
                .Index(CommentIndex)
                .Query(q => q.Term(t => t.Field(f => f.PostId.Suffix("keyword")).Value(postId.ToString()))));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch'ten post silinemedi.");
        }
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        try
        {
            await Client.DeleteAsync<CommentSearchDocument>(commentId, d => d.Index(CommentIndex));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch'ten yorum silinemedi.");
        }
    }

    public async Task<List<Guid>> SearchPostIdsAsync(string searchText, int size = 30)
    {
        var postResponse = await Client.SearchAsync<PostSearchDocument>(s => s
            .Index(PostIndex)
            .Size(size)
            .Query(q => q
                .MultiMatch(m => m
                    .Query(searchText)
                    .Fields(f => f.Field(p => p.Content).Field(p => p.UserName))
                    .Fuzziness(Fuzziness.Auto)
                    .PrefixLength(0))));

        EnsureValid(postResponse);

        var commentResponse = await Client.SearchAsync<CommentSearchDocument>(s => s
            .Index(CommentIndex)
            .Size(size)
            .Query(q => q
                .Match(m => m
                    .Field(c => c.Content)
                    .Query(searchText)
                    .Fuzziness(Fuzziness.Auto))));

        EnsureValid(commentResponse);

        return postResponse.Documents.Select(d => d.Id)
            .Concat(commentResponse.Documents.Select(d => d.PostId))
            .Distinct()
            .Take(size)
            .ToList();
    }

    public async Task<List<string>> AutoCompleteAsync(string prefix, int size = 5)
    {
        // Eski hali "completion" suggester kullanıyordu ama Content alanı completion tipinde map'lenmediği için hep hata dönüyordu.
        var response = await Client.SearchAsync<PostSearchDocument>(s => s
            .Index(PostIndex)
            .Size(size)
            .Query(q => q
                .MatchPhrasePrefix(m => m
                    .Field(p => p.Content)
                    .Query(prefix))));

        EnsureValid(response);

        return response.Documents.Select(d => d.Content).Distinct().ToList();
    }

    private static void EnsureValid(IResponse response)
    {
        // Index henüz yoksa (hiç post atılmamışsa) 404 döner: bu bir hata değil, sadece sonuç yok.
        if (response.ApiCall?.HttpStatusCode == 404) return;
        if (!response.IsValid)
            throw new InvalidOperationException($"Elasticsearch sorgusu başarısız: {response.OriginalException?.Message ?? response.ServerError?.ToString()}");
    }
}
