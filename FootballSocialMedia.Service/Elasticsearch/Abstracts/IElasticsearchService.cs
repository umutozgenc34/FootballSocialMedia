using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;
using Nest;

namespace FootballSocialMedia.Service.Elasticsearch.Abstracts;

public interface IElasticsearchService
{
    Task IndexPostAsync(Post post);
    Task IndexCommentAsync(Comment comment);
    Task DeletePostAsync(Guid postId);
    Task<ISearchResponse<Post>> SearchPostsAsync(string searchText);
    Task<ISearchResponse<Comment>> SearchCommentsAsync(string searchText);
    Task<List<PostWithCommentsResponse>> SearchAllAsync(string searchText);


}
