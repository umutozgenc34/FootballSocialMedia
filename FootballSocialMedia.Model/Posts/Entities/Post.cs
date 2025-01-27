using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Posts.Entities;

public sealed class Post : BaseEntity<Guid>, IAuditEntity
{
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
