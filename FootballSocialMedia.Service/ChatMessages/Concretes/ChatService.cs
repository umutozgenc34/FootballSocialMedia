using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.ChatMessages.Dtos.Responses;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Model.Notifications.Enums;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.ChatMessages.Abstracts;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Service.ChatHubs;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using FootballSocialMedia.Service.RabbitMQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Net;

namespace FootballSocialMedia.Service.ChatMessages.Concretes;

public class ChatService(IChatRepository chatRepository, IHubContext<ChatHub> hubContext, IMapper mapper,
    INotificationPublisher notificationPublisher, UserManager<User> userManager, MongoDbContext mongoDbContext,
    ILogger<ChatService> logger) : IChatService
{
    public const int MaxMessageLength = 2000;

    public async Task<ServiceResult<List<ChatMessageDto>>> GetChatHistory(string userId, string otherUserId)
    {
        var history = mapper.Map<List<ChatMessageDto>>(await chatRepository.GetChatHistory(userId, otherUserId));

        // 30 günden eski mesajlar MessageArchiver tarafından MongoDB'ye taşınıyor; onları da ekle.
        try
        {
            var filter = Builders<ArchivedMessage>.Filter.Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId));

            var archived = await mongoDbContext.ArchivedMessages.Find(filter).ToListAsync();

            history = archived
                .Select(a => new ChatMessageDto
                {
                    Id = Guid.TryParse(a.Id, out var id) ? id : Guid.NewGuid(),
                    SenderId = a.SenderId,
                    ReceiverId = a.ReceiverId,
                    Message = a.Message,
                    Timestamp = a.Timestamp
                })
                .Where(a => history.All(h => h.Id != a.Id))
                .Concat(history)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Arşivlenmiş mesajlar MongoDB'den okunamadı.");
        }

        return ServiceResult<List<ChatMessageDto>>.Success(history);
    }

    public async Task<ServiceResult<List<ConversationDto>>> GetConversations(string userId)
    {
        var messages = await chatRepository.GetRecentMessagesOfUserAsync(userId);

        var conversations = messages
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g =>
            {
                var last = g.First(); // mesajlar zaten yeniden eskiye sıralı
                var other = last.SenderId == userId ? last.Receiver : last.Sender;
                return new ConversationDto
                {
                    UserId = g.Key,
                    UserName = other?.UserName ?? "Unknown",
                    ProfileImageUrl = other?.Profile?.ProfileImageUrl,
                    LastMessage = last.Message,
                    LastMessageTime = last.Timestamp,
                    LastMessageIsMine = last.SenderId == userId
                };
            })
            .OrderByDescending(c => c.LastMessageTime)
            .ToList();

        return ServiceResult<List<ConversationDto>>.Success(conversations);
    }

    public async Task<ServiceResult<ChatMessageDto>> SendMessage(string senderId, string receiverId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return ServiceResult<ChatMessageDto>.Fail("Message cannot be empty.");

        if (message.Length > MaxMessageLength)
            return ServiceResult<ChatMessageDto>.Fail($"Message cannot exceed {MaxMessageLength} characters.");

        if (senderId == receiverId)
            return ServiceResult<ChatMessageDto>.Fail("You cannot send a message to yourself.");

        var sender = await userManager.FindByIdAsync(senderId);
        var receiver = await userManager.FindByIdAsync(receiverId);
        if (sender is null || receiver is null)
            return ServiceResult<ChatMessageDto>.Fail("User not found.", HttpStatusCode.NotFound);

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message.Trim(),
            Timestamp = DateTime.UtcNow
        };

        await chatRepository.AddChatMessage(chatMessage);

        var dto = new ChatMessageDto
        {
            Id = chatMessage.Id,
            SenderId = senderId,
            ReceiverId = receiverId,
            SenderName = sender.UserName,
            ReceiverName = receiver.UserName,
            Message = chatMessage.Message,
            Timestamp = chatMessage.Timestamp
        };

        // Eskiden Clients.Client(receiverId) kullanılıyordu; Client() connectionId bekler, userId değil.
        // Clients.Users(...) ise token'daki NameIdentifier claim'ine göre kullanıcının tüm bağlantılarına gönderir.
        await hubContext.Clients.Users(receiverId, senderId).SendAsync("ReceiveMessage", dto);

        notificationPublisher.SendNotification(new Notification
        {
            UserId = receiverId,
            SenderId = senderId,
            SenderUserName = sender.UserName,
            Type = NotificationType.Message,
            Content = $"{sender.UserName} sent you a message."
        });

        return ServiceResult<ChatMessageDto>.Success(dto, HttpStatusCode.Created);
    }
}
