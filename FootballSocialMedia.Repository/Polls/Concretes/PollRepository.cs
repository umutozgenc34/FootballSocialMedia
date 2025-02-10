using FootballSocialMedia.Model.Polls.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Polls.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Polls.Concretes;

public class PollRepository(AppDbContext context) : IPollRepository
{
    public async Task<List<Poll>> GetAllPollsAsync() => await context.Polls.Include(p => p.Options).ToListAsync();
    public async Task<Poll?> GetPollByIdAsync(Guid pollId) => await context.Polls.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == pollId);
    public async Task CreatePollAsync(Poll poll) => await context.Polls.AddAsync(poll);
    public async Task<bool> VoteAsync(Guid pollOptionId)
    {
        var option = await context.PollOptions.FindAsync(pollOptionId);
        if (option == null) return false;

        option.VoteCount += 1;
        return true;
    }
}
