using Asp.Versioning;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.ChatMessages.Dtos.Requests;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class ChatsController(IChatService chatService, IIdentityService identityService) : CustomBaseController
{
    /// <summary>Mesajlaştığım kişiler ve son mesajlar</summary>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations() =>
        CreateActionResult(await chatService.GetConversations(identityService.GetUserId));

    /// <summary>Benimle verilen kullanıcı arasındaki mesaj geçmişi</summary>
    [HttpGet("history/{otherUserId}")]
    public async Task<IActionResult> GetChatHistory(string otherUserId) =>
        CreateActionResult(await chatService.GetChatHistory(identityService.GetUserId, otherUserId));

    /// <summary>SignalR bağlantısı yoksa REST ile mesaj gönderme (alıcıya yine SignalR ile iletilir)</summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto) =>
        CreateActionResult(await chatService.SendMessage(identityService.GetUserId, sendMessageDto.ReceiverId, sendMessageDto.Message));
}
