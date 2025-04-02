using AutoMapper;
using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;

namespace ChatRoom.API.Services;

public class ChatEventFactory(IMapper mapper) : IChatEventFactory
{
    public ChatEvent CreateEvent(CreateEventRequest request)
    {
        return request.EventType.ToLowerInvariant() switch
        {
            "enterroom" => mapper.Map<EnterRoomEvent>(request),
            "leaveroom" => mapper.Map<LeaveRoomEvent>(request),
            "comment" => mapper.Map<CommentEvent>(request),
            "highfive" => mapper.Map<HighFiveEvent>(request),
            _ => throw new ArgumentException($"Unknown event type: {request.EventType}")
        };
    }

    public DetailedEventResponse CreateDetailedChatEventResponse(ChatEvent chatEvent)
    {
        return mapper.Map<DetailedEventResponse>(chatEvent);
    }
    
    public IEnumerable<DetailedEventResponse> CreateChatEventResponses(IEnumerable<ChatEvent> chatEvents)
    {
        return mapper.Map<IEnumerable<DetailedEventResponse>>(chatEvents);
    }

    public IEnumerable<AggregatedEventResponse> CreateAggregatedChatEventResponses(
        IEnumerable<ChatEvent> chatEvents, GranularityLevel granularity)
    {
        var aggregatedSummaries = chatEvents
            .GroupBy(e => TruncateToGranularity(e.Timestamp, granularity))
            .Select(ToAggregatedChatEventResponse)
            .OrderByDescending(s => s.PeriodStart)
            .ToList();
            
        return aggregatedSummaries;
    }
    
    private static AggregatedEventResponse ToAggregatedChatEventResponse(IGrouping<DateTime, ChatEvent> group)
    {
        return new AggregatedEventResponse
        {
            PeriodStart = group.Key,
            EntersCount = group.Count(e => e.EventType == EventType.EnterRoom),
            LeavesCount = group.Count(e => e.EventType == EventType.LeaveRoom),
            CommentsCount = group.Count(e => e.EventType == EventType.Comment),
            HighFivesCount = group.Count(e => e.EventType == EventType.HighFive),
            HighFiveDetails = group
                .Where(e => e.EventType == EventType.HighFive)
                .Select(e => $"{e.Username} high-fived {((HighFiveEvent)e).RecipientUsername}")
                .ToList()
        };
    }
    
    private static DateTime TruncateToGranularity(DateTime dateTime, GranularityLevel granularity)
    {
        return granularity switch
        {
            GranularityLevel.Minute => new DateTime(
                dateTime.Year, dateTime.Month, dateTime.Day, 
                dateTime.Hour, dateTime.Minute, 0, DateTimeKind.Utc),
                
            GranularityLevel.Hour => new DateTime(
                dateTime.Year, dateTime.Month, dateTime.Day, 
                dateTime.Hour, 0, 0, DateTimeKind.Utc),
                
            GranularityLevel.Day => new DateTime(
                dateTime.Year, dateTime.Month, dateTime.Day, 
                0, 0, 0, DateTimeKind.Utc),
            
            GranularityLevel.Month => new DateTime(
                dateTime.Year, dateTime.Month, 1, 
                0, 0, 0, DateTimeKind.Utc),
                    
            _ => throw new ArgumentOutOfRangeException(nameof(granularity))
        };
    }
}