using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Model.Polls.Dtos.Responses;
using FootballSocialMedia.Model.Polls.Entities;
using FootballSocialMedia.Repository.Polls.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Polls.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FootballSocialMedia.Service.Polls.Concretes;

public class PollService(IPollRepository pollRepository, IUnitOfWork unitOfWork, IIdGenerator idGenerator,
    IIdentityService identityService) : IPollService
{
    private static PollDto ToDto(Poll poll, Guid? myVoteOptionId) => new()
    {
        Id = poll.Id,
        Question = poll.Question,
        Options = poll.Options
            .Select(o => new PollOptionDto { Id = o.Id, OptionText = o.OptionText, VoteCount = o.VoteCount })
            .ToList(),
        TotalVotes = poll.Options.Sum(o => o.VoteCount),
        MyVoteOptionId = myVoteOptionId,
        IsExpired = poll.ExpiryDate <= DateTime.UtcNow,
        ExpiryDate = poll.ExpiryDate,
        CreatedTime = poll.CreatedTime,
        UpdatedTime = poll.UpdatedTime
    };

    public async Task<ServiceResult<PollDto>> CreateAsync(CreatePollRequest request)
    {
        var poll = new Poll
        {
            Id = idGenerator.GenerateId(),
            Question = request.Question.Trim(),
            ExpiryDate = request.ExpiryDate.ToUniversalTime(),
            Options = request.Options
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(optionText => new PollOption { Id = idGenerator.GenerateId(), OptionText = optionText.Trim() })
                .ToList()
        };

        await pollRepository.CreatePollAsync(poll);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<PollDto>.SuccessAsCreated(ToDto(poll, null), $"api/v1/polls/{poll.Id}");
    }

    public async Task<ServiceResult<List<PollDto>>> GetAllAsync()
    {
        var polls = await pollRepository.GetAllPollsAsync();
        var myVotes = await pollRepository.GetUserVotesAsync(polls.Select(p => p.Id), identityService.GetUserId);

        var dtos = polls
            .Select(p => ToDto(p, myVotes.TryGetValue(p.Id, out var optionId) ? optionId : null))
            .ToList();

        return ServiceResult<List<PollDto>>.Success(dtos, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<PollDto>> GetByIdAsync(Guid id)
    {
        var poll = await pollRepository.GetPollByIdAsync(id);
        if (poll is null)
            return ServiceResult<PollDto>.Fail("Poll not found", HttpStatusCode.NotFound);

        var myVote = await pollRepository.GetUserVoteAsync(id, identityService.GetUserId);
        return ServiceResult<PollDto>.Success(ToDto(poll, myVote?.PollOptionId), HttpStatusCode.OK);
    }

    public async Task<ServiceResult<PollDto>> VoteAsync(Guid pollId, Guid pollOptionId)
    {
        var me = identityService.GetUserId;
        var poll = await pollRepository.GetPollByIdAsync(pollId);
        if (poll is null)
            return ServiceResult<PollDto>.Fail("Poll not found.", HttpStatusCode.NotFound);

        if (poll.ExpiryDate <= DateTime.UtcNow)
            return ServiceResult<PollDto>.Fail("This poll has expired.");

        var option = poll.Options.FirstOrDefault(o => o.Id == pollOptionId);
        if (option is null)
            return ServiceResult<PollDto>.Fail("Poll option not found.", HttpStatusCode.NotFound);

        // Eskiden aynı kişi sınırsız oy verebiliyordu.
        if (await pollRepository.GetUserVoteAsync(pollId, me) is not null)
            return ServiceResult<PollDto>.Fail("You have already voted in this poll.");

        option.VoteCount += 1;
        await pollRepository.AddVoteAsync(new PollVote
        {
            Id = idGenerator.GenerateId(),
            PollId = pollId,
            PollOptionId = pollOptionId,
            UserId = me
        });

        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Aynı anda iki istek gelirse unique index (PollId, UserId) yakalar
            return ServiceResult<PollDto>.Fail("You have already voted in this poll.");
        }

        return ServiceResult<PollDto>.Success(ToDto(poll, pollOptionId));
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var poll = await pollRepository.GetPollByIdAsync(id);
        if (poll is null)
            return ServiceResult.Fail("Poll not found.", HttpStatusCode.NotFound);

        pollRepository.DeletePoll(poll);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
