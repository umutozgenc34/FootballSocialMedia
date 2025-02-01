namespace Core.Security.Services.Abstracts;

public interface IIdentityService
{
    public string GetUserId { get; }
    public string UserName { get; }
}

