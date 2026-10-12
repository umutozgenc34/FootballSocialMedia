using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Users.Dtos.Responses;

namespace FootballSocialMedia.Model.Search.Dtos;

public sealed record SearchResultDto
{
    public List<UserSummaryDto> Users { get; init; } = new();
    public List<PostDto> Posts { get; init; } = new();
    /// <summary>Arama Elasticsearch ile mi yapıldı, yoksa SQL'e mi düşüldü (ES kapalıyken)</summary>
    public string Engine { get; init; } = "elasticsearch";
}

public sealed record AutoCompleteDto
{
    public List<UserSummaryDto> Users { get; init; } = new();
    public List<string> Posts { get; init; } = new();
}
