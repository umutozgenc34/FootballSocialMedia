using Asp.Versioning;
using Core.Security.Constants;
using FootballSocialMedia.Service.Roles.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

/// <summary>Eskiden herkese açıktı: herkes kendini admin yapabiliyordu. Artık sadece admin.</summary>
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = RoleNames.Admin)]
public class RolesController(IRoleService roleService) : CustomBaseController
{
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) =>
        CreateActionResult(await roleService.CreateUserRolesAsync(userName, roles));

    [HttpPost("CreateRole/{roleName}")]
    public async Task<IActionResult> CreateRole(string roleName) =>
        CreateActionResult(await roleService.CreateRoleAsync(roleName));
}
