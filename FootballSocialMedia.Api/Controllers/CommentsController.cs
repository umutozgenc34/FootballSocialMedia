using Asp.Versioning;
using FootballSocialMedia.Model.Comments.Dtos.Requests;
using FootballSocialMedia.Service.Comments.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class CommentsController(ICommentService commentService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllComments() => CreateActionResult(await commentService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetComment([FromRoute] Guid id) => CreateActionResult(await commentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request) => CreateActionResult(await commentService.CreateAsync(request));

    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequest request) => CreateActionResult(await commentService.UpdateAsync(request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid id) => CreateActionResult(await commentService.DeleteAsync(id));

    [HttpGet("post/{postId:guid}")]
    public async Task<IActionResult> GetCommentsByPostId([FromRoute] Guid postId) => CreateActionResult(await commentService.GetCommentsByPostIdAsync(postId));

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId([FromRoute] string userId) => CreateActionResult(await commentService.GetCommentsByUserIdAsync(userId));
}
