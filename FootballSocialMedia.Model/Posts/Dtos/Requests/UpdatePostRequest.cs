using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.Posts.Dtos.Requests;

public sealed record UpdatePostRequest(Guid Id,string Content,IEnumerable<IFormFile>? ImageUrls);

