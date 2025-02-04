using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.ChatMessages.Dtos.Responses;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.ChatMessages.Abstracts;
using FootballSocialMedia.Service.ChatHubs;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace FootballSocialMedia.Service.ChatMessages.Concretes;

public class ChatService(IChatRepository chatRepository, IHubContext<ChatHub> _hubContext,IMapper mapper) : IChatService
{
    public async Task<ServiceResult<List<ChatMessageDto>>> GetChatHistory(string senderId, string receiverId)
    {
        var history = await chatRepository.GetChatHistory(senderId, receiverId);
        var historyAsDto = mapper.Map<List<ChatMessageDto>>(history);
        return ServiceResult<List<ChatMessageDto>>.Success(historyAsDto);
    }
    public async Task<ServiceResult<bool>> SendMessage(string senderId, string receiverId, string message)
    {
        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await chatRepository.AddChatMessage(chatMessage);

        if (_hubContext.Clients.Client(receiverId) != null)
        {
            await _hubContext.Clients.Client(receiverId).SendAsync("ReceiveMessage", senderId, message, chatMessage.Timestamp);
        }

        return ServiceResult<bool>.Success(true);
    }
}