using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Likes.Entities;

namespace FootballSocialMedia.Repository.Likes.Abstracts;

public interface ILikeRepository : IEfBaseRepository<Like,Guid>
{
    Task<int> GetLikeCountAsync(Guid? postId = null, Guid? commentId = null);
    Task<List<Like>> GetUserLikesAsync(string userId);
    Task RemoveLikeAsync(string userId, Guid? postId = null, Guid? commentId = null);
    Task AddLikeAsync(Like like);
}
