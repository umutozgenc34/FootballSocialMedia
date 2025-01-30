using Core.Persistence.Repositories.Abstracts;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Repository.Users.Abstracts;

public interface IUserProfileRepository : IEfBaseRepository<UserProfile,Guid>
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
}
