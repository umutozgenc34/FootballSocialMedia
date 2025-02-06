using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Posts.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Posts.Concretes;

public class PostRepository(AppDbContext context) : EfBaseRepository<AppDbContext, Post, Guid>(context), IPostRepository
{
    public override async Task<List<Post>> GetAllAsync()=> 
             await Context.Posts
            .Include(x => x.User)
            .Include(x => x.Comments)
            .Include(x=>x.Likes)
            .ToListAsync();

    public override async ValueTask<Post?> GetByIdAsync(Guid id) => await Context.Posts.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Post>> GetPostsWithCommentsAsync() => await Context.Posts.Include(x => x.User).Include(x => x.Comments).ToListAsync();
}
