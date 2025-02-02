namespace FootballSocialMedia.Model.Likes.Dtos.Requests;
public sealed record CreateLikeRequest(string UserId,Guid? PostId,Guid? CommentId);

