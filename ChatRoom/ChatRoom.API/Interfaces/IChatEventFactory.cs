using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Entities;

namespace ChatRoom.API.Interfaces;

public interface IChatEventFactory
{
    ChatEvent CreateEvent(CreateEventRequest request);
    DetailedEventResponse CreateDetailedChatEventResponse(ChatEvent chatEvent);
    IEnumerable<DetailedEventResponse> CreateChatEventResponses(IEnumerable<ChatEvent> chatEvents);
    IEnumerable<AggregatedEventResponse> CreateAggregatedChatEventResponses(IEnumerable<ChatEvent> chatEvents, GranularityLevel granularity);
}