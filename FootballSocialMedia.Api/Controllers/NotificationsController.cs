using FootballSocialMedia.Service.Notifications.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

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

