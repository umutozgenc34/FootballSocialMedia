namespace FootballSocialMedia.Model.Follows.Dtos.Requests;

// Takip eden (FollowerId) artık token'dan alınıyor.
public sealed record CreateFollowRequest(string FollowingId);
