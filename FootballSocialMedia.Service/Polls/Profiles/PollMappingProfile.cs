using AutoMapper;
using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Model.Polls.Dtos.Responses;
using FootballSocialMedia.Model.Polls.Entities;

namespace FootballSocialMedia.Service.Polls.Profiles;

public class PollMappingProfile : Profile
{
    public PollMappingProfile()
    {
        CreateMap<CreatePollRequest, Poll>();
        CreateMap<Poll, PollDto>().ReverseMap();
        CreateMap<PollOption, PollOptionDto>();
    }
}
