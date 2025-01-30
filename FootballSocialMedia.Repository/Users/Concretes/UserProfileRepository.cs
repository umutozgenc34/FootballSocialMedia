using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Users.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Users.Concretes;

public class UserProfileRepository(AppDbContext context) : EfBaseRepository<AppDbContext, UserProfile, Guid>(context), IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserIdAsync(string userId) => await context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

}
