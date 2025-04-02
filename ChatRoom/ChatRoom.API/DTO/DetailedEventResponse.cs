namespace ChatRoom.API.DTO;

public record DetailedEventResponse
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public required string EventType { get; set; }
    public required string Username { get; set; }
    public string? CommentText { get; set; }
    public string? RecipientUsername { get; set; }
}