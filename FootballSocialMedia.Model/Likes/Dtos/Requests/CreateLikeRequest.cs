namespace FootballSocialMedia.Model.Likes.Dtos.Requests;

// UserId artık token'dan alınıyor. PostId veya CommentId'den sadece biri dolu olmalı.
public sealed record CreateLikeRequest(Guid? PostId, Guid? CommentId);
