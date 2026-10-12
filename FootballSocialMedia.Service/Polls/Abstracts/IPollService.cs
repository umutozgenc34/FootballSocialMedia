using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Model.Polls.Dtos.Responses;

namespace FootballSocialMedia.Service.Polls.Abstracts;

public interface IPollService
{
    Task<ServiceResult<PollDto>> CreateAsync(CreatePollRequest request);
    Task<ServiceResult<List<PollDto>>> GetAllAsync();
    Task<ServiceResult<PollDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<PollDto>> VoteAsync(Guid pollId, Guid pollOptionId);
    Task<ServiceResult> DeleteAsync(Guid id);
}
