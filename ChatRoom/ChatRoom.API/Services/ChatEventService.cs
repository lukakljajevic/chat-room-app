using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;

namespace ChatRoom.API.Services;

public class ChatEventService(IUnitOfWork unitOfWork, ILogger<ChatEventService> logger) : IChatEventService
{
    public async Task<ChatEvent?> GetEvent(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting event by id {Id}", id.ToString());
        
        return await unitOfWork.ChatEvents.GetEventAsync(id, cancellationToken);
    }
    
    public async Task CreateEvent(ChatEvent chatEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("User {Username} is creating new {EventType} event", chatEvent.Username, chatEvent.EventType);
        
        unitOfWork.ChatEvents.AddEvent(chatEvent);
        await unitOfWork.CommitAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<ChatEvent>> GetEvents(DateTime? start, DateTime? end, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving detailed events");
        
        return await unitOfWork.ChatEvents.GetEventsAsync(start, end, cancellationToken);
    }
}