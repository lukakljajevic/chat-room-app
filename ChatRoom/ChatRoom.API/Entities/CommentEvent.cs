namespace ChatRoom.API.Entities;

public class CommentEvent : ChatEvent
{
    public required string CommentText { get; set; }
}