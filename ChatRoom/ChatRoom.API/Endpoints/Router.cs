using ChatRoom.API.DTO;
using ChatRoom.API.Helpers;

namespace ChatRoom.API.Endpoints;

public static class Router
{
    public const string GetEventRoute = "GetEvent";
    
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events/detailed", ChatEventsHandler.GetDetailedEvents)
            .WithValidation<DetailedEventsQueryParameters>()
            .Produces<IEnumerable<DetailedEventResponse>>();

        app.MapGet("/events/aggregated", ChatEventsHandler.GetAggregatedEvents)
            .WithValidation<AggregatedEventsQueryParameters>()
            .Produces<IEnumerable<AggregatedEventResponse>>();
        
        app.MapGet("/events/{id}", ChatEventsHandler.GetEvent)
            .WithName(GetEventRoute)
            .ProducesValidationProblem();
        
        app.MapPost("/events", ChatEventsHandler.CreateEvent)
            .WithValidation<CreateEventRequest>()
            .Produces(StatusCodes.Status201Created);
    }
}
