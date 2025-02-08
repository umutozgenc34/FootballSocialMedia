using Asp.Versioning;
using FootballSocialMedia.Model.ChatMessages.Dtos.Requests;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace FootballSocialMedia.Api.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]

public class ChatsController(IChatService chatService) : CustomBaseController
{
    [HttpGet("history/{senderId}/{receiverId}")]
    public async Task<IActionResult> GetChatHistory(string senderId, string receiverId) =>
        CreateActionResult(await chatService.GetChatHistory(senderId, receiverId));

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto) => 
        CreateActionResult(await chatService.SendMessage(sendMessageDto.SenderId, sendMessageDto.ReceiverId, sendMessageDto.Message));
   
}
