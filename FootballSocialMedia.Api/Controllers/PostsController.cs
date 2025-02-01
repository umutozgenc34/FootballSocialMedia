using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Service.Posts.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

public class PostsController(IPostService postService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPosts() => CreateActionResult(await postService.GetAllAsync());
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPost([FromRoute] Guid id) => CreateActionResult(await postService.GetByIdAsync(id));
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request) => CreateActionResult(await postService
        .CreateAsync(request));

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdatePost([FromForm] UpdatePostRequest request) => CreateActionResult
        (await postService.UpdateAsync(request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id) => CreateActionResult(await postService.DeleteAsync(id));

    [HttpGet("with-comments")]
    public async Task<IActionResult> GetPostsWithComments()
        => CreateActionResult(await postService.GetPostsWithCommentsAsync());

}
