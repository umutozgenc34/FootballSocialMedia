using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Repository.Comments.Abstracts;
using FootballSocialMedia.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Comments.Concretes;

public class CommentRepository(AppDbContext context) : EfBaseRepository<AppDbContext, Comment, Guid>(context), ICommentRepository
{
    public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId) => await Context.Comments.
        Where(x => x.PostId == postId).
        OrderByDescending(x => x.CreatedTime).
        ToListAsync();

    public async Task<List<Comment>> GetCommentsByUserIdAsync(string userId) => await Context.Comments.
        Where(c => c.UserId == userId).
        OrderByDescending(c => c.CreatedTime).
        ToListAsync();

}
