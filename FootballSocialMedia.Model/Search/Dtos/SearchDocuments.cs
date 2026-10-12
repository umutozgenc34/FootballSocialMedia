namespace FootballSocialMedia.Model.Search.Dtos;

// Elasticsearch'e entity'nin kendisi yerine bu hafif dokümanlar yazılır.
// (Entity'yi yazınca navigation property'ler üzerinden kullanıcı şifre hash'i dahil her şey index'e gidiyordu.)
public sealed class PostSearchDocument
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string? UserName { get; set; }
    public DateTime CreatedTime { get; set; }
}

public sealed class CommentSearchDocument
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string? UserName { get; set; }
    public DateTime CreatedTime { get; set; }
}
