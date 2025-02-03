using AutoMapper;
using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Model.Follows.Dtos.Responses;
using FootballSocialMedia.Model.Follows.Entities;

namespace FootballSocialMedia.Service.Follows.Profiles;

public class FollowMappingProfile : Profile
{
    public FollowMappingProfile()
    {
        CreateMap<CreateFollowRequest, Follow>();
        CreateMap<Follow, FollowDto>().ReverseMap();
    }
}
