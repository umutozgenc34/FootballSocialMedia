using Core.Infrastructure.Elasticsearch;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Comments.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using Nest;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _elasticsearchClient;

    public ElasticsearchService(ElasticsearchClient elasticsearchClient)
    {
        _elasticsearchClient = elasticsearchClient;
    }

    public async Task IndexPostAsync(Post post)
    {
        var indexResponse = await _elasticsearchClient.GetClient().IndexDocumentAsync(post);

        if (!indexResponse.IsValid)
        {
            throw new Exception($"Failed to index post: {indexResponse.DebugInformation}");
        }
    }

    public async Task DeletePostAsync(Guid postId)
    {
        var deleteResponse = await _elasticsearchClient.GetClient().DeleteAsync<Post>(postId);

        if (!deleteResponse.IsValid)
        {
            throw new Exception($"Failed to delete post from Elasticsearch: {deleteResponse.DebugInformation}");
        }
    }

    public async Task IndexCommentAsync(Comment comment)
    {
        var indexResponse = await _elasticsearchClient.GetClient().IndexDocumentAsync(comment);

        if (!indexResponse.IsValid)
        {
            throw new Exception($"Failed to index comment: {indexResponse.DebugInformation}");
        }
    }

    public async Task<ISearchResponse<Post>> SearchPostsAsync(string searchText)
    {
        return await _elasticsearchClient.GetClient().SearchAsync<Post>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Query(searchText)
                    .Fields(f => f.Field(p => p.Content))
                    .Fuzziness(Fuzziness.Auto)
                    .PrefixLength(0)
                )
            )
            //.Sort(srt => srt.Descending(p => p.Likes.Count))
        );
    }

   public async Task<ISearchResponse<Comment>> SearchCommentsAsync(string searchText)
    {
        return await _elasticsearchClient.GetClient().SearchAsync<Comment>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Query(searchText)
                    .Fields(f => f.Field(c => c.Content))
                    .Fuzziness(Fuzziness.Auto)
                    .PrefixLength(0)
                )
            )
            //.Sort(srt => srt.Descending(c => c.Likes.Count))
        );
    }

    public async Task<List<PostWithCommentsResponse>> SearchAllAsync(string searchText)
    {
        var postResults = await SearchPostsAsync(searchText);
        var commentResults = await SearchCommentsAsync(searchText);

        var postWithCommentsList = postResults.Documents.Select(post => new PostWithCommentsResponse
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrls = post.ImageUrls,
            VideoUrls = post.VideoUrls,
            Username = post.User?.UserName,
            CreatedTime = post.CreatedTime,
            UpdatedTime = post.UpdatedTime,

            Comments = commentResults.Documents
                .Where(comment => comment.PostId == post.Id)
                .Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedTime = comment.CreatedTime,
                    UpdatedTime = comment.UpdatedTime,
                    Username = comment.User.UserName,
                    PostContent = comment.Post.Content
                }).ToList()
        }).ToList();

        return postWithCommentsList;
    }

    public async Task<ISearchResponse<Post>> AutoCompleteAsync(string prefix)
    {
        return await _elasticsearchClient.GetClient().SearchAsync<Post>(s => s
            .Suggest(su => su
                .Completion("post-suggestions", c => c
                    .Field(p => p.Content)
                    .Prefix(prefix)
                    .Fuzzy(f => f.Fuzziness(Fuzziness.Auto))
                )
            )
        );
    }

}
