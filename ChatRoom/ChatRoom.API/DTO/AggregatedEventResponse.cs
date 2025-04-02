namespace ChatRoom.API.DTO;

public record AggregatedEventResponse
{
    public DateTime PeriodStart { get; set; }
    public int EntersCount { get; set; }
    public int LeavesCount { get; set; }
    public int CommentsCount { get; set; }
    public int HighFivesCount { get; set; }
    public List<string> HighFiveDetails { get; set; } = [];
}