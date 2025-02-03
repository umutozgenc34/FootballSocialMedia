using FootballSocialMedia.Model.Follows.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Follows.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Follows.Concretes;

public class FollowRepository(AppDbContext context) : IFollowRepository
{
    public async Task<List<Follow>> GetAllAsync() => await context.Set<Follow>().ToListAsync();

    public async Task<Follow?> GetByIdAsync(string followerId, string followingId) => await context.Set<Follow>()
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    public async Task<List<Follow>> GetByFollowerIdAsync(string followerId) => await context.Set<Follow>()
            .Where(f => f.FollowerId == followerId)
            .ToListAsync();

    public async Task<List<Follow>> GetByFollowingIdAsync(string followingId) => await context.Set<Follow>()
            .Where(f => f.FollowingId == followingId)
            .ToListAsync();

    public async Task AddAsync(Follow follow) => await context.Set<Follow>().AddAsync(follow);
    
    public void Update(Follow follow) => context.Set<Follow>().Update(follow);

    public void Delete(Follow follow) => context.Set<Follow>().Remove(follow);

}