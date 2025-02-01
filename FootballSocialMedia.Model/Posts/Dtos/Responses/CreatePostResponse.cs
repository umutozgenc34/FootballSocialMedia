namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record CreatePostResponse(Guid Id,string UserId,string Content,List<string> ImageUrls,List<string> VideoUrls,DateTime CreatedTime);
