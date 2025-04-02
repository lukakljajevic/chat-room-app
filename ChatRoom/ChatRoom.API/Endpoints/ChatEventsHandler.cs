using ChatRoom.API.DTO;
using ChatRoom.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.API.Endpoints;

public static class ChatEventsHandler
{
    public static async Task<IResult> GetDetailedEvents([AsParameters] DetailedEventsQueryParameters parameters, IChatEventService eventService, IChatEventFactory chatEventFactory, CancellationToken cancellationToken)
    {
        var chatEvents = (await eventService.GetEvents(parameters.StartDate, parameters.EndDate, cancellationToken)).ToList();
        var detailedEventResponses = chatEventFactory.CreateChatEventResponses(chatEvents);
        return Results.Ok(detailedEventResponses);
    }

    public static async Task<IResult> GetAggregatedEvents([AsParameters] AggregatedEventsQueryParameters parameters,
        IChatEventService eventService, IChatEventFactory chatEventFactory, CancellationToken cancellationToken)
    {
        var chatEvents = (await eventService.GetEvents(parameters.StartDate, parameters.EndDate, cancellationToken)).ToList();
        var aggregatedEvents =
            chatEventFactory.CreateAggregatedChatEventResponses(chatEvents, parameters.GetGranularityLevel());
        return Results.Ok(aggregatedEvents);
    }

    public static async Task<IResult> GetEvent(Guid id, IChatEventService chatEventService, IChatEventFactory chatEventFactory, CancellationToken cancellationToken)
    {
        var chatEvent = await chatEventService.GetEvent(id, cancellationToken);
        if (chatEvent is null)
        {
            return Results.NotFound("Chat event not found.");
        }

        var detailedChatEventResponse = chatEventFactory.CreateDetailedChatEventResponse(chatEvent);
        return Results.Ok(detailedChatEventResponse);
    }

    public static async Task<IResult> CreateEvent([FromBody] CreateEventRequest createEventRequest, IChatEventService chatEventService, IChatEventFactory chatEventFactory, CancellationToken cancellationToken)
    {
        var newChatEvent = chatEventFactory.CreateEvent(createEventRequest);
        await chatEventService.CreateEvent(newChatEvent, cancellationToken);
        var response = chatEventFactory.CreateDetailedChatEventResponse(newChatEvent);
        return Results.CreatedAtRoute(Router.GetEventRoute, new { id = response.Id }, response);
    }
}