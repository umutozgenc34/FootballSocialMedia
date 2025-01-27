using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Posts.Entities;

namespace FootballSocialMedia.Repository.Posts.Abstracts;

public interface IPostRepository : IEfBaseRepository<Post,Guid>
{
}
