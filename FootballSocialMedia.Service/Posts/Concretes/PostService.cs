using AutoMapper;
using CloudinaryDotNet;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Posts.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FootballSocialMedia.Service.Posts.Concretes;

public class PostService(IPostRepository postRepository,IMapper mapper,IUnitOfWork unitOfWork,IIdGenerator idGenerator,ICloudinaryService cloudinaryService) : IPostService
{
    public async Task<ServiceResult<CreatePostResponse>> CreateAsync(CreatePostRequest request)
    {
        List<string> imageUrls = new List<string>();
        List<string> videoUrls = new List<string>();

        
        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            imageUrls = await cloudinaryService.UploadImages(request.ImageUrls, "post-images");
        }

        
        if (request.VideoUrls != null && request.VideoUrls.Any())
        {
            videoUrls = await cloudinaryService.UploadVideos(request.VideoUrls, "post-videos");
        }

        var post = mapper.Map<Post>(request);
        post.Id = idGenerator.GenerateId();
        post.ImageUrls = imageUrls;
        post.VideoUrls = videoUrls;

        await postRepository.AddAsync(post);
        await unitOfWork.SaveChangesAsync();

        var postAsDto = mapper.Map<CreatePostResponse>(post);

        return ServiceResult<CreatePostResponse>.SuccessAsCreated(postAsDto, $"api/posts/{postAsDto.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null)
        {
            return ServiceResult.Fail("Post not found", System.Net.HttpStatusCode.NotFound);
        }
        postRepository.Delete(post);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<PostDto>>> GetAllAsync()
    {
        var posts = await postRepository.GetAll().ToListAsync();
        var postsAsDto = mapper.Map<List<PostDto>>(posts);

        return ServiceResult<List<PostDto>>.Success(postsAsDto,HttpStatusCode.OK);
      
    }

    public async Task<ServiceResult<PostDto>> GetByIdAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null)
        {
            return ServiceResult<PostDto>.Fail("Post not found", HttpStatusCode.NotFound);
        }

        var postAsDto = mapper.Map<PostDto>(post);
        return ServiceResult<PostDto>.Success(postAsDto,HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(UpdatePostRequest request)
    {

        var post = await postRepository.GetByIdAsync(request.Id);
        if (post is null)
        {
            return ServiceResult.Fail("Post not found.", HttpStatusCode.NotFound);
        }

        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            var imageUrls = await cloudinaryService.UploadImages(request.ImageUrls, "post-images");
            post.ImageUrls = imageUrls;
        }

        if (request.VideoUrls != null && request.VideoUrls.Any())
        {
            var videoUrls = await cloudinaryService.UploadVideos(request.VideoUrls, "post-videos");
            post.VideoUrls = videoUrls;
        }

        post.Content = request.Content;
        post.UpdatedTime = DateTime.UtcNow;

        postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }


}
