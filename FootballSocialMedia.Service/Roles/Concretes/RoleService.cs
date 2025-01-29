using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Service.Roles.Abstracts;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace FootballSocialMedia.Service.Roles.Concretes;

public class RoleService : IRoleService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles)
    {
        if (roles == null || !roles.Any())
        {
            return ServiceResult.Fail("The roles to be assigned should be specified.");
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }
        foreach (var role in roles)
        {

            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    return ServiceResult.Fail($"An error occurred while creating the role: {role}");
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
            {
                return ServiceResult.Fail($"An error occurred while assigning the role to the user.: {role}");
            }
        }

        return ServiceResult.Success("The roles has been successfully added.", HttpStatusCode.Created);
    }

    public async Task<ServiceResult> CreateRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return ServiceResult.Fail("Role name cannot be empty.");
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return ServiceResult.Fail("This role already exists.");
        }

        var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
        if (!result.Succeeded)
        {
            return ServiceResult.Fail("An error occurred while creating the role.");
        }

        return ServiceResult.Success("Role successfully created.", HttpStatusCode.Created);
    }

}

