using Microsoft.AspNetCore.Http;

namespace FootballSocialMedia.Model.News.Dtos.Requests;

// Görsel güncellemede opsiyonel: gönderilmezse eski görsel kalır.
public sealed record UpdateNewsRequest(Guid Id, string Title, string Content, string Source, IFormFile? ImageUrl);
