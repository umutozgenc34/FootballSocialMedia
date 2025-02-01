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
        CreateMap<CommentDto, Comment>().ReverseMap();
        CreateMap<Comment, CreateCommentResponse>();
    }
}
