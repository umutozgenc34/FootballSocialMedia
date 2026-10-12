using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.Users.Dtos.Requests;

// Güncellenen kullanıcı token'dan alınıyor. multipart/form-data ile gönderilir.
public sealed record UpdateUserRequest(
    string? Email,
    string? UserName,
    string? FavoriteTeam,
    string? Bio,
    IFormFile? ProfileImage);
