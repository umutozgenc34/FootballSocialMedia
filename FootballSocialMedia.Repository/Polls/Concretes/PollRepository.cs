using FootballSocialMedia.Model.Polls.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Polls.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Polls.Concretes;

public class PollRepository(AppDbContext context) : IPollRepository
{
    public async Task<List<Poll>> GetAllPollsAsync() => await context.Polls
        .Include(p => p.Options)
        .OrderByDescending(p => p.CreatedTime)
        .ToListAsync();

    public async Task<Poll?> GetPollByIdAsync(Guid pollId) =>
        await context.Polls.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == pollId);

    public async Task CreatePollAsync(Poll poll) => await context.Polls.AddAsync(poll);

    public void DeletePoll(Poll poll)
    {
        // PollVote -> PollOption ilişkisi NoAction olduğu için önce oyları siliyoruz
        context.PollVotes.RemoveRange(context.PollVotes.Where(v => v.PollId == poll.Id));
        context.Polls.Remove(poll);
    }

    public async Task<PollVote?> GetUserVoteAsync(Guid pollId, string userId) =>
        await context.PollVotes.FirstOrDefaultAsync(v => v.PollId == pollId && v.UserId == userId);

    public async Task<Dictionary<Guid, Guid>> GetUserVotesAsync(IEnumerable<Guid> pollIds, string userId)
    {
        var ids = pollIds.ToList();
        return await context.PollVotes
            .Where(v => v.UserId == userId && ids.Contains(v.PollId))
            .ToDictionaryAsync(v => v.PollId, v => v.PollOptionId);
    }

    public async Task AddVoteAsync(PollVote vote) => await context.PollVotes.AddAsync(vote);
}
