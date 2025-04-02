using ChatRoom.API.Common;
using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;
using ChatRoom.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatRoom.Tests;

public class ChatEventServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IChatEventRepository> _mockRepository;
    private readonly ChatEventService _service;

    public ChatEventServiceTests()
    {
        _mockRepository = new Mock<IChatEventRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _mockUnitOfWork.Setup(uow => uow.ChatEvents).Returns(_mockRepository.Object);
        
        var mockLogger = new Mock<ILogger<ChatEventService>>();
        _service = new ChatEventService(_mockUnitOfWork.Object, mockLogger.Object);
    }
    
    [Fact]
    public async Task GetEvent_CallsRepository_AndReturnsResult()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var expectedEvent = new EnterRoomEvent
        {
            Id = eventId,
            Username = "testuser",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.EnterRoom
        };
        
        _mockRepository
            .Setup(repo => repo.GetEventAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvent);
        
        // Act
        var result = await _service.GetEvent(eventId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventId, result.Id);
        Assert.Equal(expectedEvent.Username, result.Username);
        Assert.Equal(EventType.EnterRoom, result.EventType);
        
        _mockRepository.Verify(repo => repo.GetEventAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetEvent_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        _mockRepository
            .Setup(repo => repo.GetEventAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChatEvent?)null);
        
        // Act
        var result = await _service.GetEvent(nonExistentId);
        
        // Assert
        Assert.Null(result);
        _mockRepository.Verify(repo => repo.GetEventAsync(nonExistentId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateEvent_AddsToRepository_AndCommits()
    {
        // Arrange
        var newEvent = new CommentEvent
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.Comment,
            CommentText = "Test comment"
        };
        
        _mockUnitOfWork
            .Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act
        await _service.CreateEvent(newEvent);
        
        // Assert
        _mockRepository.Verify(repo => repo.AddEvent(newEvent), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetEvents_CallsRepository_WithCorrectParameters()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow;
        
        var expectedEvents = new List<ChatEvent>
        {
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = startDate.AddHours(2),
                EventType = EventType.EnterRoom
            },
            new CommentEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = startDate.AddHours(3),
                EventType = EventType.Comment,
                CommentText = "Hello"
            }
        };
        
        _mockRepository
            .Setup(repo => repo.GetEventsAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvents);
        
        // Act
        var results = await _service.GetEvents(startDate, endDate);
        var resultsList = results.ToList();
        
        // Assert
        Assert.Equal(2, resultsList.Count);
        _mockRepository.Verify(repo => repo.GetEventsAsync(startDate, endDate, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetEvents_WithNoDateParameters_PassesNullToRepository()
    {
        // Arrange
        var expectedEvents = new List<ChatEvent>
        {
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = DateTime.UtcNow,
                EventType = EventType.EnterRoom
            }
        };
        
        _mockRepository
            .Setup(repo => repo.GetEventsAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvents);
        
        // Act
        var results = await _service.GetEvents(null, null);
        
        // Assert
        var resultsList = results.ToList();
        Assert.Single(resultsList);
        _mockRepository.Verify(repo => repo.GetEventsAsync(null, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}