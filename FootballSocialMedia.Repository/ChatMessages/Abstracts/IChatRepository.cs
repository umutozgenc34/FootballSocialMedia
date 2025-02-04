using FootballSocialMedia.Model.ChatMessages.Entities;

namespace FootballSocialMedia.Repository.ChatMessages.Abstracts;

public interface IChatRepository
{
    Task<List<ChatMessage>> GetChatHistory(string senderId, string receiverId);
    Task AddChatMessage(ChatMessage chatMessage);

}
