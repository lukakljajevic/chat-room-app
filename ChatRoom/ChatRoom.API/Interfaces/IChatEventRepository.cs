using ChatRoom.API.Entities;

namespace ChatRoom.API.Interfaces;

public interface IChatEventRepository
{
    Task<ChatEvent?> GetEventAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatEvent>> GetEventsAsync(DateTime? start = null, DateTime? end = null, CancellationToken cancellationToken = default);
    void AddEvent(ChatEvent chatEvent);
}