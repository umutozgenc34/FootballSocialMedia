namespace FootballSocialMedia.Model.Posts.Dtos.Requests;

public sealed record UpdatePostRequest(Guid Id,string Content,string? ImageUrl);

