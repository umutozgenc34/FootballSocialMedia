using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.News.Entities;

public class News : BaseEntity<Guid>, IAuditEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string? ImageUrl { get; set; } 
    public DateTime CreatedTime { get ; set; }
    public DateTime UpdatedTime { get; set; }
}
