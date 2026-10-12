using Asp.Versioning;
using FootballSocialMedia.Service.Notifications.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class NotificationsController(INotificationService notificationService) : CustomBaseController
{
    /// <summary>Giriş yapan kullanıcının son bildirimleri (yeniden eskiye)</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int take = 50) =>
        CreateActionResult(await notificationService.GetMyNotificationsAsync(take));

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount() => CreateActionResult(await notificationService.GetUnreadCountAsync());

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(string notificationId) =>
        CreateActionResult(await notificationService.MarkAsReadAsync(notificationId));

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead() => CreateActionResult(await notificationService.MarkAllAsReadAsync());
}
