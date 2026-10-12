using Asp.Versioning;
using Core.Security.Constants;
using FootballSocialMedia.Model.Users.Dtos.Requests;
using FootballSocialMedia.Service.Users.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class UsersController(IUserService userService) : CustomBaseController
{
    /// <summary>Giriş yapan kullanıcının profili</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe() => CreateActionResult(await userService.GetMeAsync());

    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetProfile([FromRoute] string id) => CreateActionResult(await userService.GetProfileAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q) => CreateActionResult(await userService.SearchAsync(q));

    /// <summary>Takip etmediğim kullanıcılardan öneriler</summary>
    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] int take = 5) => CreateActionResult(await userService.GetSuggestionsAsync(take));

    [HttpGet("by-username/{userName}")]
    public async Task<IActionResult> GetByUserName([FromRoute] string userName) => CreateActionResult(await userService.GetUserByNameAsync(userName));

    [HttpGet]
    public async Task<IActionResult> GetAllUsers() => CreateActionResult(await userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id) => CreateActionResult(await userService.GetByIdAsync(id));

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteUser([FromRoute] string id) => CreateActionResult(await userService.DeleteAsync(id));

    /// <summary>Kendi profilimi güncelle (multipart/form-data: UserName, Email, FavoriteTeam, Bio, ProfileImage)</summary>
    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateMe([FromForm] UpdateUserRequest request) => CreateActionResult(await userService.UpdateAsync(request));
}
