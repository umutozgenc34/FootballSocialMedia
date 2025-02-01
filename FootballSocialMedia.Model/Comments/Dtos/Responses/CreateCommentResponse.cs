namespace FootballSocialMedia.Model.Comments.Dtos.Responses;

public sealed record CreateCommentResponse(Guid Id,string Content,string UserId,Guid PostId,DateTime CreatedTime);
