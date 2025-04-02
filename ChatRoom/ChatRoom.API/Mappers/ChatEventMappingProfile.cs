using AutoMapper;
using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Entities;

namespace ChatRoom.API.Mappers;

public class ChatEventMappingProfile : Profile
{
    public ChatEventMappingProfile()
    {
        CreateMap<CreateEventRequest, ChatEvent>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Timestamp, opt => 
                opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<CreateEventRequest, EnterRoomEvent>()
            .IncludeBase<CreateEventRequest, ChatEvent>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(_ => EventType.EnterRoom));
            
        CreateMap<CreateEventRequest, LeaveRoomEvent>()
            .IncludeBase<CreateEventRequest, ChatEvent>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(_ => EventType.LeaveRoom));

        CreateMap<CreateEventRequest, CommentEvent>()
            .IncludeBase<CreateEventRequest, ChatEvent>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(_ => EventType.Comment))
            .ForMember(dest => dest.CommentText, opt => opt.MapFrom(o => o.CommentText));
            
            
        CreateMap<CreateEventRequest, HighFiveEvent>()
            .IncludeBase<CreateEventRequest, ChatEvent>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(_ => EventType.HighFive))
            .ForMember(dest => dest.RecipientUsername, opt => opt.MapFrom(o => o.Recipient));

        CreateMap<ChatEvent, DetailedEventResponse>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType.ToString().ToLower()));

        CreateMap<CommentEvent, DetailedEventResponse>()
            .IncludeBase<ChatEvent, DetailedEventResponse>();
        
        CreateMap<HighFiveEvent, DetailedEventResponse>()
            .IncludeBase<ChatEvent, DetailedEventResponse>();
    }
}