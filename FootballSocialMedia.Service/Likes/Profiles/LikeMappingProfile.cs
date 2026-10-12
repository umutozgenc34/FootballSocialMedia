using AutoMapper;
using FootballSocialMedia.Model.Likes.Dtos.Responses;
using FootballSocialMedia.Model.Likes.Entities;

namespace FootballSocialMedia.Service.Likes.Profiles;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<Like, LikeDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"))
            .ForMember(dest => dest.PostContent, opt => opt.MapFrom(src => src.Post != null ? src.Post.Content : null))
            .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => src.Comment != null ? src.Comment.Content : null))
            .ForMember(dest => dest.LikeCount, opt => opt.Ignore());
    }
}
