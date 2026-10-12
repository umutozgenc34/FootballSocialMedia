using FootballSocialMedia.Model.ChatMessages.Entities;

namespace FootballSocialMedia.Repository.ChatMessages.Abstracts;

public interface IChatRepository
{
    Task<List<ChatMessage>> GetChatHistory(string userId, string otherUserId);
    Task<ChatMessage?> GetByIdAsync(Guid id);
    /// <summary>Kullanıcının dahil olduğu son mesajlar (sohbet listesi için)</summary>
    Task<List<ChatMessage>> GetRecentMessagesOfUserAsync(string userId, int take = 500);
    Task AddChatMessage(ChatMessage chatMessage);
}
