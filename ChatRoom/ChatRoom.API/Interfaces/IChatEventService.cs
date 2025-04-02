using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Entities;

namespace ChatRoom.API.Interfaces;

public interface IChatEventService
{
    Task<ChatEvent?> GetEvent(Guid id, CancellationToken cancellationToken = default);
    Task CreateEvent(ChatEvent createEventRequest, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatEvent>> GetEvents(DateTime? start, DateTime? end, CancellationToken cancellationToken = default);
}