using Core.Persistence.Repositories.Concretes;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Posts.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Posts.Concretes;

public class PostRepository(AppDbContext context) : EfBaseRepository<AppDbContext, Post, Guid>(context), IPostRepository
{
    public async Task<List<Post>> GetPostsWithCommentsAsync() => await Context.Posts.Include(x => x.Comments).ToListAsync();
}
