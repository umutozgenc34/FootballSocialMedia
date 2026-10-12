using FootballSocialMedia.Model.Polls.Entities;

namespace FootballSocialMedia.Repository.Polls.Abstracts;

public interface IPollRepository
{
    Task<List<Poll>> GetAllPollsAsync();
    Task<Poll?> GetPollByIdAsync(Guid pollId);
    Task CreatePollAsync(Poll poll);
    void DeletePoll(Poll poll);
    Task<PollVote?> GetUserVoteAsync(Guid pollId, string userId);
    Task<Dictionary<Guid, Guid>> GetUserVotesAsync(IEnumerable<Guid> pollIds, string userId);
    Task AddVoteAsync(PollVote vote);
}
