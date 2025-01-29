using Core.CrossCuttingConcerns.Response;

namespace FootballSocialMedia.Service.Roles.Abstracts;

public interface IRoleService
{
    Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles);
}
