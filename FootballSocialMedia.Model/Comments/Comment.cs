using Core.Persistence.Entity;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Model.Comments;

public sealed class Comment : BaseEntity<Guid>, IAuditEntity
{
    public string Content { get; set; } = default!;
    public string UserId { get; set; } = default!; 
    public User User { get; set; } = default!; 
    public Guid PostId { get; set; } 
    public Post Post { get; set; } = default!;
    public DateTime CreatedTime { get;set; }
    public DateTime UpdatedTime { get; set; }

    public List<Like> Likes { get; set; } = new List<Like>();
}
