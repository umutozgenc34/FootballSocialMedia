using FootballSocialMedia.Service.Roles.Abstracts;
using FootballSocialMedia.Service.Users.Concretes;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

public class RolesController(IRoleService roleService) : CustomBaseController
{
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => CreateActionResult(await roleService
       .CreateUserRolesAsync(userName, roles));

    [HttpPost("CreateRole/{roleName}")]
    public async Task<IActionResult> CreateRole(string roleName) =>
    CreateActionResult(await roleService.CreateRoleAsync(roleName));

}
