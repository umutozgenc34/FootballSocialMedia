using Asp.Versioning;
using FootballSocialMedia.Service.Roles.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class RolesController(IRoleService roleService) : CustomBaseController
{
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => CreateActionResult(await roleService
       .CreateUserRolesAsync(userName, roles));

    [HttpPost("CreateRole/{roleName}")]
    public async Task<IActionResult> CreateRole(string roleName) =>
    CreateActionResult(await roleService.CreateRoleAsync(roleName));

}
