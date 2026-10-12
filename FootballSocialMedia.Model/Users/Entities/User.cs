using Core.Persistence.Entity;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Posts.Entities;
using Microsoft.AspNetCore.Identity;

namespace FootballSocialMedia.Model.Users.Entities;

public sealed class User : IdentityUser, IAuditEntity
{
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    public UserProfile? Profile { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    
}
