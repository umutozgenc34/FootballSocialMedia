using FootballSocialMedia.Model.Users.Dtos.Responses;

namespace FootballSocialMedia.Model.Users.Dtos.Requests;

public sealed record UpdateUserRequest(string Id,string? Email,string? UserName,UserProfileDto? UserProfile);