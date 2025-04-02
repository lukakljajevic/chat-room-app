using ChatRoom.API.Common;

namespace ChatRoom.API.Entities;

public abstract class ChatEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public required string Username { get; set; }
    public EventType EventType { get; set; }
}