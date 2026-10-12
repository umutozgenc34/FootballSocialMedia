using FootballSocialMedia.Model.Follows.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Follows.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Follows.Concretes;

public class FollowRepository(AppDbContext context) : IFollowRepository
{
    private IQueryable<Follow> WithUsers() => context.Set<Follow>()
        .Include(f => f.Follower).ThenInclude(u => u.Profile)
        .Include(f => f.Following).ThenInclude(u => u.Profile);

    public async Task<List<Follow>> GetAllAsync() => await context.Set<Follow>().ToListAsync();

    public async Task<Follow?> GetByIdAsync(string followerId, string followingId) => await context.Set<Follow>()
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    public async Task<List<Follow>> GetByFollowerIdAsync(string followerId) => await WithUsers()
            .Where(f => f.FollowerId == followerId)
            .OrderByDescending(f => f.CreatedTime)
            .ToListAsync();

    public async Task<List<Follow>> GetByFollowingIdAsync(string followingId) => await WithUsers()
            .Where(f => f.FollowingId == followingId)
            .OrderByDescending(f => f.CreatedTime)
            .ToListAsync();

    public Task<int> CountFollowersAsync(string userId) => context.Set<Follow>().CountAsync(f => f.FollowingId == userId);

    public Task<int> CountFollowingAsync(string userId) => context.Set<Follow>().CountAsync(f => f.FollowerId == userId);

    public Task<List<string>> GetFollowingIdsAsync(string followerId) => context.Set<Follow>()
        .Where(f => f.FollowerId == followerId)
        .Select(f => f.FollowingId)
        .ToListAsync();

    public async Task AddAsync(Follow follow) => await context.Set<Follow>().AddAsync(follow);
    public void Update(Follow follow) => context.Set<Follow>().Update(follow);
    public void Delete(Follow follow) => context.Set<Follow>().Remove(follow);
}
