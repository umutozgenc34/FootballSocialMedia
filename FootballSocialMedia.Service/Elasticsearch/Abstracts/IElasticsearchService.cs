using FootballSocialMedia.Model.Search.Dtos;

namespace FootballSocialMedia.Service.Elasticsearch.Abstracts;

public interface IElasticsearchService
{
    /// <summary>Index'leme hataları yutulur ve loglanır; ES kapalı diye post atılamaması olmasın.</summary>
    Task IndexPostAsync(PostSearchDocument post);
    Task IndexCommentAsync(CommentSearchDocument comment);
    Task DeletePostAsync(Guid postId);
    Task DeleteCommentAsync(Guid commentId);

    /// <summary>Metinle eşleşen post id'leri (post içeriği veya yorumları eşleşenler). ES'e ulaşılamazsa exception fırlatır.</summary>
    Task<List<Guid>> SearchPostIdsAsync(string searchText, int size = 30);

    /// <summary>Prefix ile başlayan post içerikleri (autocomplete). ES'e ulaşılamazsa exception fırlatır.</summary>
    Task<List<string>> AutoCompleteAsync(string prefix, int size = 5);
}
