using AutoMapper;
using FootballSocialMedia.Model.ChatMessages.Dtos.Responses;
using FootballSocialMedia.Model.ChatMessages.Entities;

namespace FootballSocialMedia.Service.ChatMessages.Profiles;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>()
       .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.UserName))
       .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.UserName)) 
       .ReverseMap(); ;
    }
}
