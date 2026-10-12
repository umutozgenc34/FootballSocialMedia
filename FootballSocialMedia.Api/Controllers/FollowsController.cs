using Asp.Versioning;
using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Service.Follows.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class FollowsController(IFollowService followService) : CustomBaseController
{
    /// <summary>Giriş yapan kullanıcı, FollowingId'deki kullanıcıyı takip eder</summary>
    [HttpPost]
    public async Task<IActionResult> Follow([FromBody] CreateFollowRequest request) =>
        CreateActionResult(await followService.FollowAsync(request));

    [HttpDelete("{followingId}")]
    public async Task<IActionResult> Unfollow([FromRoute] string followingId) =>
        CreateActionResult(await followService.UnfollowAsync(followingId));

    /// <summary>userId'nin takip ettiği kişiler</summary>
    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing([FromRoute] string userId) =>
        CreateActionResult(await followService.GetFollowingAsync(userId));

    /// <summary>userId'nin takipçileri</summary>
    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers([FromRoute] string userId) =>
        CreateActionResult(await followService.GetFollowersAsync(userId));
}
