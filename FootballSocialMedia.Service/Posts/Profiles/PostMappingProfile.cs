using AutoMapper;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;

namespace FootballSocialMedia.Service.Posts.Profiles;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User != null && src.User.Profile != null ? src.User.Profile.ProfileImageUrl : null))
            .ForMember(dest => dest.FavoriteTeam, opt => opt.MapFrom(src => src.User != null && src.User.Profile != null ? src.User.Profile.FavoriteTeam : null))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.IsLikedByMe, opt => opt.Ignore());

        CreateMap<Post, PostWithCommentsResponse>()
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"));
    }
}
