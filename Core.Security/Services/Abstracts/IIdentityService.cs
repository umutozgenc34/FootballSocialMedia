namespace Core.Security.Services.Abstracts;

public interface IIdentityService
{
    /// <summary>Token'daki kullanıcı Id'si (NameIdentifier). Giriş yapılmamışsa boş string.</summary>
    string GetUserId { get; }
    string UserName { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
