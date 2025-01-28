using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Services.CloudinaryService.Abstracts;

public interface ICloudinaryService
{
    Task<List<string>> UploadImages(IEnumerable<IFormFile> files, string folder);
    Task<List<string>> UploadVideos(IEnumerable<IFormFile> files, string folder);
}
