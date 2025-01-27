using AutoMapper;
using Core.CrossCuttingConcerns.Response;
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

public class PostService(IPostRepository postRepository,IMapper mapper,IUnitOfWork unitOfWork,IIdGenerator idGenerator) : IPostService
{
    public async Task<ServiceResult<CreatePostResponse>> CreateAsync(CreatePostRequest request)
    {
        var post = mapper.Map<Post>(request);
        post.Id = idGenerator.GenerateId();

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
        var isPostIdExist = await postRepository.Where(x => x.Id != request.Id).AnyAsync();

        if (isPostIdExist is true)
        {
            return ServiceResult.Fail("There is a post with the same ID.", HttpStatusCode.BadRequest);
        }

        var post = await postRepository.GetByIdAsync(request.Id);
        if (post is null)
        {
            return ServiceResult.Fail("Post not found.", HttpStatusCode.NotFound);
        }

        var courseAsDto = mapper.Map(request, post);

        postRepository.Update(post);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
