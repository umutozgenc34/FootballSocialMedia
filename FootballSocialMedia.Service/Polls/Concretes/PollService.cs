using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Model.Polls.Dtos.Responses;
using FootballSocialMedia.Model.Polls.Entities;
using FootballSocialMedia.Repository.Polls.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Polls.Abstracts;
using System.Net;

namespace FootballSocialMedia.Service.Polls.Concretes;

public class PollService(IPollRepository pollRepository, IMapper mapper, IUnitOfWork unitOfWork,IIdGenerator idGenerator) : IPollService
{
    public async Task<ServiceResult<PollDto>> CreateAsync(CreatePollRequest request)
    {
        //var poll = mapper.Map<Poll>(request);
        var poll = new Poll
        {
            Id = idGenerator.GenerateId(),
            Question = request.Question,
            ExpiryDate = request.ExpiryDate,
            Options = request.Options.Select(optionText => new PollOption { OptionText = optionText }).ToList()
        };
        
        await pollRepository.CreatePollAsync(poll);
        await unitOfWork.SaveChangesAsync();

        var pollAsDto = mapper.Map<PollDto>(poll);

        return ServiceResult<PollDto>.SuccessAsCreated(pollAsDto, $"api/polls/{pollAsDto.Id}");
    }

    public async Task<ServiceResult<List<PollDto>>> GetAllAsync()
    {
        var polls = await pollRepository.GetAllPollsAsync();
        var pollsAsDto = mapper.Map<List<PollDto>>(polls);

        return ServiceResult<List<PollDto>>.Success(pollsAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<PollDto>> GetByIdAsync(Guid id)
    {
        var poll = await pollRepository.GetPollByIdAsync(id);
        if (poll is null)
        {
            return ServiceResult<PollDto>.Fail("Poll not found", HttpStatusCode.NotFound);
        }

        var pollAsDto = mapper.Map<PollDto>(poll);
        return ServiceResult<PollDto>.Success(pollAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> VoteAsync(Guid pollOptionId)
    {
        var success = await pollRepository.VoteAsync(pollOptionId);
        if (!success)
        {
            return ServiceResult.Fail("Poll option not found or vote failed.", HttpStatusCode.NotFound);
        }

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}