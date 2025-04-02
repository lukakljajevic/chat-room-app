namespace ChatRoom.API.Entities;

public class HighFiveEvent : ChatEvent
{
    public required string RecipientUsername { get; set; }
}