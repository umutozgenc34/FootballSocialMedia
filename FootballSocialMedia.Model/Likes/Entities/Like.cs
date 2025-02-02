using Core.Persistence.Entity;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Model.Likes.Entities;

public sealed class Like : BaseEntity<Guid>
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public Guid? PostId { get; set; }
    public Post? Post { get; set; }

    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }

}