using FootballSocialMedia.Model.Follows.Entities;

namespace FootballSocialMedia.Repository.Follows.Abstracts;

public interface IFollowRepository
{
    Task<List<Follow>> GetAllAsync();
    Task<Follow?> GetByIdAsync(string followerId, string followingId);
    Task<List<Follow>> GetByFollowerIdAsync(string followerId);
    Task<List<Follow>> GetByFollowingIdAsync(string followingId);
    Task<int> CountFollowersAsync(string userId);
    Task<int> CountFollowingAsync(string userId);
    Task<List<string>> GetFollowingIdsAsync(string followerId);
    Task AddAsync(Follow follow);
    void Update(Follow follow);
    void Delete(Follow follow);
}
