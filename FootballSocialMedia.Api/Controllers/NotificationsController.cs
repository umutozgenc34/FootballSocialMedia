using Asp.Versioning;
using FootballSocialMedia.Service.Notifications.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]

public class NotificationsController(INotificationService notificationService): CustomBaseController
{
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetNotificationsByUserIdAsync(string userId) =>
        Ok(await notificationService.GetNotificationsByUserIdAsync(userId));
   

    [HttpPut("markAsRead/{notificationId}")]
    public async Task<IActionResult> MarkAsReadAsync(string notificationId)
    {
        await notificationService.MarkAsReadAsync(notificationId);
        return Ok("Notification marked as read.");
    }
}

