using Core.Persistence.Entity;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Model.Posts.Entities;

public sealed class Post : BaseEntity<Guid>, IAuditEntity
{
    public string Content { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; } = new List<string>();
    public List<string> VideoUrls { get; set; } = new List<string>();
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Like> Likes { get; set; } = new List<Like>();
}
