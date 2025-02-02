using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Likes.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Likes.Concretes;

public class LikeRepository(AppDbContext context) : EfBaseRepository<AppDbContext, Like, Guid>(context), ILikeRepository
{
    public async Task<int> GetLikeCountAsync(Guid? postId = null, Guid? commentId = null) => await Context.Likes.CountAsync(x =>
        (x.PostId == postId || x.CommentId == commentId));
   

    public async Task<List<Like>> GetUserLikesAsync(string userId) =>  await Context.Likes
        .Where(l => l.UserId == userId)
        .ToListAsync();

    public async Task AddLikeAsync(Like like) =>
      await Context.Likes.AddAsync(like);

    public async Task RemoveLikeAsync(string userId, Guid? postId = null, Guid? commentId = null)
    {
        var like = await Context.Likes
           .FirstOrDefaultAsync(l => l.UserId == userId && (l.PostId == postId || l.CommentId == commentId));

        if (like is not null)
            Context.Likes.Remove(like);
    }
}
