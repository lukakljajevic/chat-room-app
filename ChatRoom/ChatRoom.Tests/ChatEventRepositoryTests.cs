using ChatRoom.API.Common;
using ChatRoom.API.Data;
using ChatRoom.API.Entities;
using ChatRoom.API.Repositories;
using MockQueryable.Moq;
using Moq;

namespace ChatRoom.Tests;

public class ChatEventRepositoryTests
{
    private readonly Mock<ChatRoomDbContext> _mockContext;
    private readonly ChatEventRepository _repository;

    public ChatEventRepositoryTests()
    {
        var sampleEvents = new List<ChatEvent>
        {
            new EnterRoomEvent 
            { 
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Username = "user1",
                EventType = EventType.EnterRoom
            },
            new CommentEvent 
            { 
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                Timestamp = DateTime.UtcNow.AddDays(-1),
                Username = "user2",
                EventType = EventType.Comment,
                CommentText = "Hello everyone!"
            },
            new LeaveRoomEvent 
            { 
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                Timestamp = DateTime.UtcNow,
                Username = "user3",
                EventType = EventType.LeaveRoom
            }
        };
        
        var mockSet = sampleEvents.AsQueryable().BuildMockDbSet();
        
        _mockContext = new Mock<ChatRoomDbContext>();
        _mockContext.Setup(c => c.Events).Returns(mockSet.Object);
        
        _repository = new ChatEventRepository(_mockContext.Object);
    }

    [Fact]
    public async Task GetEventAsync_WithExistingId_ReturnsChatEvent()
    {
        // Arrange
        var existingId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var result = await _repository.GetEventAsync(existingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingId, result.Id);
    }

    [Fact]
    public async Task GetEventAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        // Act
        var result = await _repository.GetEventAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEventsAsync_WithNoParameters_ReturnsAllEventsOrderedByTimestampDescending()
    {
        // Act
        var results = await _repository.GetEventsAsync();
        var resultsList = results.ToList();

        // Assert
        Assert.Equal(3, resultsList.Count);
        
        // Verify descending order
        Assert.True(resultsList[0].Timestamp >= resultsList[1].Timestamp);
        Assert.True(resultsList[1].Timestamp >= resultsList[2].Timestamp);
    }

    [Fact]
    public async Task GetEventsAsync_WithStartDate_ReturnsEventsAfterStartDate()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1.5);

        // Act
        var results = await _repository.GetEventsAsync(start: startDate);
        var resultsList = results.ToList();

        // Assert
        Assert.Equal(2, resultsList.Count);
        Assert.All(resultsList, e => Assert.True(e.Timestamp >= startDate));
    }

    [Fact]
    public async Task GetEventsAsync_WithEndDate_ReturnsEventsBeforeEndDate()
    {
        // Arrange
        var endDate = DateTime.UtcNow.AddDays(-0.5);

        // Act
        var results = await _repository.GetEventsAsync(end: endDate);
        var resultsList = results.ToList();

        // Assert
        Assert.Equal(2, resultsList.Count);
        Assert.All(resultsList, e => Assert.True(e.Timestamp <= endDate));
    }

    [Fact]
    public async Task GetEventsAsync_WithStartAndEndDate_ReturnsEventsBetweenDates()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1.5);
        var endDate = DateTime.UtcNow.AddDays(-0.5);

        // Act
        var results = await _repository.GetEventsAsync(start: startDate, end: endDate);
        var resultsList = results.ToList();

        // Assert
        Assert.Single(resultsList);
        Assert.All(resultsList, e => Assert.True(e.Timestamp >= startDate && e.Timestamp <= endDate));
    }

    [Fact]
    public void AddEvent_AddsEventToContext()
    {
        // Arrange
        var newEvent = new HighFiveEvent
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Timestamp = DateTime.UtcNow.AddHours(1),
            Username = "user4",
            EventType = EventType.HighFive,
            RecipientUsername = "user2"
        };

        // Act
        _repository.AddEvent(newEvent);

        // Assert
        _mockContext.Verify(m => m.Events.Add(It.Is<ChatEvent>(e => e.Id == newEvent.Id)), Times.Once);
    }
}