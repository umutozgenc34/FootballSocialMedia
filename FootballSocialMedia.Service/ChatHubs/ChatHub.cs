using Core.Infrastructure.SignalR;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace FootballSocialMedia.Service.ChatHubs;

public class ChatHub(IChatService chatService) : BaseChatHub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string senderId, string receiverId, string message)
    {
        var result = await chatService.SendMessage(senderId, receiverId, message);

        if (result.IsSuccess)
        {
            await Clients.Client(receiverId).SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
        }
    }
}