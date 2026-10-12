using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.ChatMessages.Abstracts;
using FootballSocialMedia.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.ChatMessages.Concretes;

public class ChatRepository(AppDbContext context) : IChatRepository
{
    public async Task<List<ChatMessage>> GetChatHistory(string userId, string otherUserId) => await context.ChatMessages
            .Include(c => c.Sender)
            .Include(c => c.Receiver)
            .Where(c => (c.SenderId == userId && c.ReceiverId == otherUserId) ||
                        (c.SenderId == otherUserId && c.ReceiverId == userId))
            .OrderBy(c => c.Timestamp)
            .ToListAsync();

    public async Task<ChatMessage?> GetByIdAsync(Guid id) => await context.ChatMessages
        .Include(c => c.Sender)
        .Include(c => c.Receiver)
        .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<ChatMessage>> GetRecentMessagesOfUserAsync(string userId, int take = 500) => await context.ChatMessages
        .Include(c => c.Sender).ThenInclude(u => u.Profile)
        .Include(c => c.Receiver).ThenInclude(u => u.Profile)
        .Where(c => c.SenderId == userId || c.ReceiverId == userId)
        .OrderByDescending(c => c.Timestamp)
        .Take(take)
        .ToListAsync();

    public async Task AddChatMessage(ChatMessage chatMessage)
    {
        context.ChatMessages.Add(chatMessage);
        await context.SaveChangesAsync();
    }
}
