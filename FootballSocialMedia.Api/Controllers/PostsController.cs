using Asp.Versioning;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Service.Posts.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class PostsController(IPostService postService) : CustomBaseController
{
    /// <summary>Tüm postlar (Redis cache'li)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPosts() => CreateActionResult(await postService.GetAllAsync());

    /// <summary>Sayfalı akış. mode=all → herkes, mode=following → takip ettiklerim + ben</summary>
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] string mode = "all", [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        CreateActionResult(await postService.GetFeedAsync(mode, page, pageSize));

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPosts([FromRoute] string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        CreateActionResult(await postService.GetUserPostsAsync(userId, page, pageSize));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPost([FromRoute] Guid id) => CreateActionResult(await postService.GetByIdAsync(id));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request) => CreateActionResult(await postService.CreateAsync(request));

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdatePost([FromForm] UpdatePostRequest request) => CreateActionResult(await postService.UpdateAsync(request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id) => CreateActionResult(await postService.DeleteAsync(id));

    [HttpGet("with-comments")]
    public async Task<IActionResult> GetPostsWithComments() => CreateActionResult(await postService.GetPostsWithCommentsAsync());
}
