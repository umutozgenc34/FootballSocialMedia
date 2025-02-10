using FootballSocialMedia.Model.Polls.Entities;

namespace FootballSocialMedia.Repository.Polls.Abstracts;

public interface IPollRepository
{
    Task<List<Poll>> GetAllPollsAsync();
    Task<Poll?> GetPollByIdAsync(Guid pollId);
    Task CreatePollAsync(Poll poll);
    Task<bool> VoteAsync(Guid pollOptionId);
}