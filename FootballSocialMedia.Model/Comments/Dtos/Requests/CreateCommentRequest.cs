namespace FootballSocialMedia.Model.Comments.Dtos.Requests;

// UserId artık token'dan alınıyor.
public sealed record CreateCommentRequest(string Content, Guid PostId);
