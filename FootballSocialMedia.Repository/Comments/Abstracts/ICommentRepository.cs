using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Comments;

namespace FootballSocialMedia.Repository.Comments.Abstracts;

public interface ICommentRepository : IEfBaseRepository<Comment,Guid>
{
    Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<List<Comment>> GetCommentsByUserIdAsync(string userId);
}
