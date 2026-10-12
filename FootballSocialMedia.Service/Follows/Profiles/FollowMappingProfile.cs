using AutoMapper;
using FootballSocialMedia.Model.Follows.Dtos.Responses;
using FootballSocialMedia.Model.Follows.Entities;

namespace FootballSocialMedia.Service.Follows.Profiles;

public class FollowMappingProfile : Profile
{
    public FollowMappingProfile()
    {
        CreateMap<Follow, FollowDto>()
            .ForMember(d => d.FollowerUserName, o => o.MapFrom(s => s.Follower != null ? s.Follower.UserName : null))
            .ForMember(d => d.FollowerProfileImageUrl, o => o.MapFrom(s => s.Follower != null && s.Follower.Profile != null ? s.Follower.Profile.ProfileImageUrl : null))
            .ForMember(d => d.FollowingUserName, o => o.MapFrom(s => s.Following != null ? s.Following.UserName : null))
            .ForMember(d => d.FollowingProfileImageUrl, o => o.MapFrom(s => s.Following != null && s.Following.Profile != null ? s.Following.Profile.ProfileImageUrl : null));
    }
}
