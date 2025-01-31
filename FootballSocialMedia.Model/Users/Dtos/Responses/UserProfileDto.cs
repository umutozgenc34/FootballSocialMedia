using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.Users.Dtos.Responses;

public sealed record UserProfileDto(string? FavoriteTeam,string? Bio,IFormFile? ProfileImageUrl);
