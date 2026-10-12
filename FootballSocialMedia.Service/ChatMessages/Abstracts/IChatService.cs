using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.ChatMessages.Dtos.Responses;

namespace FootballSocialMedia.Service.ChatMessages.Abstracts;

public interface IChatService
{
    /// <summary>İki kullanıcı arasındaki tüm mesajlar (SQL'deki güncel + MongoDB'deki arşivlenmiş).</summary>
    Task<ServiceResult<List<ChatMessageDto>>> GetChatHistory(string userId, string otherUserId);

    /// <summary>Kullanıcının mesajlaştığı kişiler ve son mesajlar.</summary>
    Task<ServiceResult<List<ConversationDto>>> GetConversations(string userId);

    /// <summary>Mesajı kaydeder ve SignalR ile hem alıcıya hem gönderenin diğer sekmelerine iletir.</summary>
    Task<ServiceResult<ChatMessageDto>> SendMessage(string senderId, string receiverId, string message);
}
