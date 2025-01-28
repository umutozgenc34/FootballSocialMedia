namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record CreatePostResponse(Guid Id,string Content,List<string> ImageUrls,DateTime CreatedTime);
