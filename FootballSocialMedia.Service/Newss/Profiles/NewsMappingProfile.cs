using AutoMapper;
using FootballSocialMedia.Model.News.Dtos.Requests;
using FootballSocialMedia.Model.News.Dtos.Responses;
using FootballSocialMedia.Model.News.Entities;

namespace FootballSocialMedia.Service.Newss.Profiles;

public class NewsMappingProfile : Profile
{
    public NewsMappingProfile()
    {
        CreateMap<CreateNewsRequest, News>();
        CreateMap<UpdateNewsRequest, News>();
        CreateMap<News, NewsDto>().ReverseMap() ;
    }
}
