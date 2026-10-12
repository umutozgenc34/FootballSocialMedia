using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.News.Dtos.Requests;
using FootballSocialMedia.Model.News.Dtos.Responses;
using FootballSocialMedia.Model.News.Entities;
using FootballSocialMedia.Repository.Newss.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Newss.Abstracts;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FootballSocialMedia.Service.Newss.Concretes;

public class NewsService(INewsRepository newsRepository, IMapper mapper, IUnitOfWork unitOfWork, IIdGenerator idGenerator, ICloudinaryService cloudinaryService) : INewsService
{
    public async Task<ServiceResult<NewsDto>> CreateAsync(CreateNewsRequest request)
    {
        List<string> imageUrls = new List<string>();

        if (request.ImageUrl != null)
        {
            imageUrls = await cloudinaryService.UploadImages(new List<IFormFile> { request.ImageUrl }, "news-images");
        }

        var news = mapper.Map<News>(request);
        news.Id = idGenerator.GenerateId();
        news.ImageUrl = imageUrls.FirstOrDefault();

        await newsRepository.AddAsync(news);
        await unitOfWork.SaveChangesAsync();

        var newsAsDto = mapper.Map<NewsDto>(news);

        return ServiceResult<NewsDto>.SuccessAsCreated(newsAsDto, $"api/news/{newsAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var news = await newsRepository.GetByIdAsync(id);
        if (news is null)
        {
            return ServiceResult.Fail("News not found", HttpStatusCode.NotFound);
        }

        newsRepository.Delete(news);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<NewsDto>>> GetAllAsync()
    {
        var news = (await newsRepository.GetAllAsync()).OrderByDescending(n => n.CreatedTime).ToList();
        var newsAsDto = mapper.Map<List<NewsDto>>(news);

        return ServiceResult<List<NewsDto>>.Success(newsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<NewsDto>> GetByIdAsync(Guid id)
    {
        var news = await newsRepository.GetByIdAsync(id);
        if (news is null)
        {
            return ServiceResult<NewsDto>.Fail("News not found", HttpStatusCode.NotFound);
        }

        var newsAsDto = mapper.Map<NewsDto>(news);
        return ServiceResult<NewsDto>.Success(newsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateNewsRequest request)
    {
        var news = await newsRepository.GetByIdAsync(request.Id);
        if (news is null)
        {
            return ServiceResult.Fail("News not found.", HttpStatusCode.NotFound);
        }

        if (request.ImageUrl != null)
        {
            var imageUrls = await cloudinaryService.UploadImages(new List<IFormFile> { request.ImageUrl }, "news-images");
            news.ImageUrl = imageUrls.FirstOrDefault(); 
        }

        news.Title = request.Title;
        news.Content = request.Content;
        news.Source = request.Source;

        newsRepository.Update(news);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}