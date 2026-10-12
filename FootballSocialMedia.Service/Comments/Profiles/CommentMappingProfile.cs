using AutoMapper;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Comments.Dtos.Responses;

namespace FootballSocialMedia.Service.Comments.Profiles;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        // Servis artık projeksiyon (Select) kullanıyor; bu map PostWithCommentsResponse içindeki yorumlar için.
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown"))
            .ForMember(dest => dest.PostContent, opt => opt.MapFrom(src => src.Post != null ? src.Post.Content : null))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User != null && src.User.Profile != null ? src.User.Profile.ProfileImageUrl : null))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.IsLikedByMe, opt => opt.Ignore());
    }
}
