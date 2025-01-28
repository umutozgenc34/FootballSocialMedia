using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Core.Infrastructure.Services.CloudinaryService.Concretes;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly Account _account;
    private readonly CloudinarySettings _cloudinarySettings;
    public CloudinaryService(IOptions<CloudinarySettings> cloudOptions)
    {
        _cloudinarySettings = cloudOptions.Value;
        _account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
        _cloudinary = new Cloudinary(_account);
    }
    public async Task<List<string>> UploadImages(IEnumerable<IFormFile> files, string folder)
    {
        var imageUrls = new List<string>();

        foreach (var file in files)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new CloudinaryException("Image upload failed.");
                }
            }
        }

        return imageUrls;
    }

    public async Task<List<string>> UploadVideos(IEnumerable<IFormFile> files, string folder)
    {
        var videoUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    videoUrls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new CloudinaryException("Video upload failed.");
                }
            }
        }

        return videoUrls;
    }
}
