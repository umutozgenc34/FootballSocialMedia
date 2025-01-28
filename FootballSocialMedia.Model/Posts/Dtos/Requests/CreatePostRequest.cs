using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.Posts.Dtos.Requests;

public sealed record CreatePostRequest(string Content, IEnumerable<IFormFile>? ImageUrls,IEnumerable<IFormFile>? VideoUrls);
