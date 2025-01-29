using AutoMapper;
using FootballSocialMedia.Model.Users.Dtos.Responses;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Service.Users.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDto, User>().ReverseMap();
    }
}
