using Asp.Versioning;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Service.Likes.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class LikesController(ILikeService likeService) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Like([FromBody] CreateLikeRequest request) => CreateActionResult(await likeService.LikeAsync(request));

    [HttpDelete]
    public async Task<IActionResult> Unlike([FromQuery] Guid? postId = null, [FromQuery] Guid? commentId = null) =>
        CreateActionResult(await likeService.UnlikeAsync(postId, commentId));

    [HttpGet("count")]
    public async Task<IActionResult> GetLikeCount([FromQuery] Guid? postId = null, [FromQuery] Guid? commentId = null) =>
        CreateActionResult(await likeService.GetLikeCountAsync(postId, commentId));

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserLikes([FromRoute] string userId) => CreateActionResult(await likeService.GetUserLikesAsync(userId));
}
