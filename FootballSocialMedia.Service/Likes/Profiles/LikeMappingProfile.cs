using AutoMapper;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Model.Likes.Dtos.Responses;
using FootballSocialMedia.Model.Likes.Entities;

namespace FootballSocialMedia.Service.Likes.Profiles;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<CreateLikeRequest, Like>();
        CreateMap<Like, LikeDto>().ReverseMap();
    }
}
