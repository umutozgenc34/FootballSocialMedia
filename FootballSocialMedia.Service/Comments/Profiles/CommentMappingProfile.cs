using AutoMapper;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Comments.Dtos.Requests;
using FootballSocialMedia.Model.Comments.Dtos.Responses;

namespace FootballSocialMedia.Service.Comments.Profiles;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();

        CreateMap<Comment, CommentDto>()
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"))
        .ForMember(dest => dest.PostContent, opt => opt.MapFrom(src => src.Post != null ? src.Post.Content : "No Content"));

        CreateMap<CommentDto, Comment>(); //nadir
        CreateMap<Comment, CreateCommentResponse>()
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"))
        .ForMember(dest => dest.PostContent, opt => opt.MapFrom(src => src.Post != null ? src.Post.Content : "No Content")); 
    }
}
