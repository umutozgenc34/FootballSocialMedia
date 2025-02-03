using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Service.Follows.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

public class FollowsController(IFollowService followService) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Follow([FromBody] CreateFollowRequest request) =>
        CreateActionResult(await followService.FollowAsync(request));

    [HttpDelete]
    public async Task<IActionResult> Unfollow([FromQuery] string followerId, [FromQuery] string followingId) => 
        CreateActionResult(await followService.UnfollowAsync(followerId, followingId));


    [HttpGet("following/{followerId}")]
    public async Task<IActionResult> GetFollowing([FromRoute] string followerId) => // follower ID ye ait takip edilen kisiler
        CreateActionResult(await followService.GetFollowingAsync(followerId));


    [HttpGet("followers/{followingId}")]
    public async Task<IActionResult> GetFollowers([FromRoute] string followingId) // following Id ye ait takipçiler
        => CreateActionResult(await followService.GetFollowersAsync(followingId));
    
}
