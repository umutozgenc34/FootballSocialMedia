namespace FootballSocialMedia.Model.Comments.Dtos.Requests;

public sealed record CreateCommentRequest(string Content,string UserId,Guid PostId);
