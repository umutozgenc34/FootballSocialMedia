using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.Posts.Dtos.Requests;

// UserId artık token'dan alınıyor.
public sealed record CreatePostRequest(string Content, IEnumerable<IFormFile>? ImageUrls, IEnumerable<IFormFile>? VideoUrls);
