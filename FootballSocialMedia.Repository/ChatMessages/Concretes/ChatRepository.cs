using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.ChatMessages.Abstracts;
using FootballSocialMedia.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.ChatMessages.Concretes;

public class ChatRepository(AppDbContext context) : IChatRepository
{

    public async Task<List<ChatMessage>> GetChatHistory(string senderId, string receiverId) => await context.ChatMessages
            .Include(c => c.Sender)    
            .Include(c => c.Receiver)  
            .Where(c => (c.SenderId == senderId && c.ReceiverId == receiverId) ||
                        (c.SenderId == receiverId && c.ReceiverId == senderId))
            .OrderBy(c => c.Timestamp) 
            .ToListAsync();


    public async Task AddChatMessage(ChatMessage chatMessage)
    {
        context.ChatMessages.Add(chatMessage);
        await context.SaveChangesAsync();
    }
}