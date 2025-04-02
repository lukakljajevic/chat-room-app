using ChatRoom.API.Data;
using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.API.Repositories;

public class ChatEventRepository(ChatRoomDbContext context) : IChatEventRepository
{
    public async Task<ChatEvent?> GetEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Events.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ChatEvent>> GetEventsAsync(DateTime? start = null, DateTime? end = null, CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsQueryable();

        if (start.HasValue)
        {
            query = query.Where(e => e.Timestamp >= start.Value);
        }
        
        if (end.HasValue)
        {
            query = query.Where(e => e.Timestamp <= end.Value);
        }
            
        return await query.OrderByDescending(e => e.Timestamp).ToListAsync(cancellationToken);
    }

    public void AddEvent(ChatEvent chatEvent)
    {
        context.Events.Add(chatEvent);
    }
}