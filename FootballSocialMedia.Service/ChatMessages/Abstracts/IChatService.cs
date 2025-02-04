using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.ChatMessages.Dtos.Responses;

namespace FootballSocialMedia.Service.ChatMessages.Abstracts;

public interface IChatService
{
    Task<ServiceResult<List<ChatMessageDto>>> GetChatHistory(string senderId, string receiverId);
    Task<ServiceResult<bool>> SendMessage(string senderId, string receiverId, string message);
}
