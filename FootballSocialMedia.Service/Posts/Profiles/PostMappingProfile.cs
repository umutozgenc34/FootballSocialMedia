using AutoMapper;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;

namespace FootballSocialMedia.Service.Posts.Profiles;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<CreatePostRequest, Post>();
        CreateMap<UpdatePostRequest, Post>();
        CreateMap<Post, PostDto>().ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown")).ReverseMap();
        CreateMap<Post, CreatePostResponse>();
        CreateMap<Post, PostWithCommentsResponse>().ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
       .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown")).ReverseMap(); 
    }
}
