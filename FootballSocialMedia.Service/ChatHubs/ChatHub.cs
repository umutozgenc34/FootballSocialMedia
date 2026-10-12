using Core.Infrastructure.SignalR;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FootballSocialMedia.Service.ChatHubs;

/// <summary>
/// Gerçek zamanlı mesajlaşma + bildirim hub'ı. Endpoint: /chathub
/// İstemci token'ı query string ile gönderir: /chathub?access_token=...
/// Sunucu → istemci olayları: "ReceiveMessage"(ChatMessageDto), "ReceiveNotification"(Notification)
/// </summary>
[Authorize]
public class ChatHub(IChatService chatService) : BaseChatHub
{
    public async Task SendMessage(string receiverId, string message)
    {
        var senderId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(senderId))
            throw new HubException("Unauthorized");

        var result = await chatService.SendMessage(senderId, receiverId, message);
        if (result.IsFail)
            throw new HubException(string.Join(", ", result.ErrorMessage!));
        // Mesajın alıcıya iletilmesini ChatService yapıyor (Clients.Users).
    }
}
