namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record CreatePostResponse(Guid Id,string Content,string ImageUrl,DateTime CreatedTime);
