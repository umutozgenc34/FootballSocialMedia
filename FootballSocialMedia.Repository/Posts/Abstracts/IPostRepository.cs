using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Posts.Entities;

namespace FootballSocialMedia.Repository.Posts.Abstracts;

public interface IPostRepository : IEfBaseRepository<Post,Guid>
{
    Task<List<Post>> GetPostsWithCommentsAsync();

    /// <summary>
    /// Post'u; post'a ve yorumlarına ait beğenilerle birlikte siler.
    /// (Likes tablosundaki FK'lar NO ACTION olduğu için beğenisi olan post doğrudan silinemiyordu.)
    /// </summary>
    Task DeleteWithRelationsAsync(Post post);
}
