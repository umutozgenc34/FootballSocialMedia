using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Likes.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Likes.Concretes;

public class LikeRepository(AppDbContext context) : EfBaseRepository<AppDbContext, Like, Guid>(context), ILikeRepository
{
    // Not: Eskiden (PostId == postId || CommentId == commentId) kullanılıyordu.
    // postId verilip commentId null olunca "CommentId == null" tüm post beğenilerini de eşleştiriyordu → yanlış sayım/çakışma.
    public async Task<int> GetLikeCountAsync(Guid? postId = null, Guid? commentId = null)
    {
        if (postId is not null) return await Context.Likes.CountAsync(x => x.PostId == postId);
        if (commentId is not null) return await Context.Likes.CountAsync(x => x.CommentId == commentId);
        return 0;
    }

    public async Task<List<Like>> GetUserLikesAsync(string userId) => await Context.Likes
        .Where(l => l.UserId == userId)
        .Include(l => l.User)
        .Include(l => l.Post)
        .Include(l => l.Comment)
        .ToListAsync();

    public async Task<Like?> GetUserLikeAsync(string userId, Guid? postId = null, Guid? commentId = null)
    {
        if (postId is not null)
            return await Context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        if (commentId is not null)
            return await Context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.CommentId == commentId);
        return null;
    }

    public async Task AddLikeAsync(Like like) => await Context.Likes.AddAsync(like);
}
