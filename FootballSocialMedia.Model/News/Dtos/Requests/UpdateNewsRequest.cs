using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.News.Dtos.Requests;

public sealed record UpdateNewsRequest(Guid Id,string Title, string Content, string Source, IFormFile ImageUrl);