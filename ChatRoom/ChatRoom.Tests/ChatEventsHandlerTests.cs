using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Endpoints;
using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ChatRoom.Tests;

public class ChatEventsHandlerTests
{
    private readonly Mock<IChatEventService> _mockEventService = new();
    private readonly Mock<IChatEventFactory> _mockEventFactory = new();

    [Fact]
    public async Task GetEvent_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var chatEvent = new EnterRoomEvent
        {
            Id = eventId,
            Username = "user1",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.EnterRoom
        };
        
        var detailedResponse = new DetailedEventResponse
        {
            Id = eventId,
            Username = "user1",
            Timestamp = chatEvent.Timestamp,
            EventType = "EnterRoom"
        };

        _mockEventService
            .Setup(service => service.GetEvent(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatEvent);
            
        _mockEventFactory
            .Setup(factory => factory.CreateDetailedChatEventResponse(chatEvent))
            .Returns(detailedResponse);

        // Act
        var result = await ChatEventsHandler.GetEvent(
            eventId,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<DetailedEventResponse>>(result);
        Assert.Equal(detailedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetEvent_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _mockEventService
            .Setup(service => service.GetEvent(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChatEvent?)null);

        // Act
        var result = await ChatEventsHandler.GetEvent(
            nonExistentId,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFound<string>>(result);
        Assert.Equal("Chat event not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetDetailedEvents_ReturnsOkResultWithEvents()
    {
        // Arrange
        var parameters = new DetailedEventsQueryParameters
        {
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow
        };

        var events = new List<ChatEvent>
        {
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = DateTime.UtcNow.AddHours(-2),
                EventType = EventType.EnterRoom
            }
        };

        var responses = new List<DetailedEventResponse>
        {
            new()
            {
                Id = events[0].Id,
                Username = "user1",
                Timestamp = events[0].Timestamp,
                EventType = "EnterRoom"
            }
        };

        _mockEventService
            .Setup(service => service.GetEvents(parameters.StartDate, parameters.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        _mockEventFactory
            .Setup(factory => factory.CreateChatEventResponses(events))
            .Returns(responses);

        // Act
        var result = await ChatEventsHandler.GetDetailedEvents(
            parameters,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<DetailedEventResponse>>>(result);
        Assert.Equal(responses, okResult.Value);
    }

    [Fact]
    public async Task GetAggregatedEvents_ReturnsOkResultWithAggregatedEvents()
    {
        // Arrange
        var parameters = new AggregatedEventsQueryParameters
        {
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Granularity = "hour"
        };

        var events = new List<ChatEvent>
        {
            new EnterRoomEvent { Id = Guid.NewGuid(), Username = "user1", Timestamp = DateTime.UtcNow.AddDays(-1), EventType = EventType.EnterRoom },
            new CommentEvent { Id = Guid.NewGuid(), Username = "user1", Timestamp = DateTime.UtcNow.AddHours(-5), EventType = EventType.Comment, CommentText = "Test comment"}
        };

        var aggregatedResponses = new List<AggregatedEventResponse>
        {
            new()
            {
                PeriodStart = DateTime.UtcNow.Date,
                EntersCount = 1,
                CommentsCount = 1,
                LeavesCount = 0,
                HighFivesCount = 0,
                HighFiveDetails = []
            }
        };

        _mockEventService
            .Setup(service => service.GetEvents(parameters.StartDate, parameters.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(events);

        _mockEventFactory
            .Setup(factory => factory.CreateAggregatedChatEventResponses(events, parameters.GetGranularityLevel()))
            .Returns(aggregatedResponses);

        // Act
        var result = await ChatEventsHandler.GetAggregatedEvents(
            parameters,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<AggregatedEventResponse>>>(result);
        Assert.Equal(aggregatedResponses, okResult.Value);
    }

    [Fact]
    public async Task CreateEvent_ReturnsCreatedAtRouteResult()
    {
        // Arrange
        var createRequest = new CreateEventRequest
        {
            EventType = "Comment",
            Username = "user1",
            CommentText = "Hello world"
        };

        var newEvent = new CommentEvent
        {
            Id = Guid.NewGuid(),
            Username = "user1",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.Comment,
            CommentText = "Hello world"
        };

        var detailedResponse = new DetailedEventResponse
        {
            Id = newEvent.Id,
            Username = "user1",
            Timestamp = newEvent.Timestamp,
            EventType = "Comment",
            CommentText = "Hello world"
        };

        _mockEventFactory
            .Setup(factory => factory.CreateEvent(createRequest))
            .Returns(newEvent);

        _mockEventFactory
            .Setup(factory => factory.CreateDetailedChatEventResponse(newEvent))
            .Returns(detailedResponse);

        // Act
        var result = await ChatEventsHandler.CreateEvent(
            createRequest,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRoute<DetailedEventResponse>>(result);
        Assert.Equal(detailedResponse, createdResult.Value);
        
        _mockEventService.Verify(service => 
            service.CreateEvent(newEvent, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task CreateEvent_WithHighFive_CreatesHighFiveEvent()
    {
        // Arrange
        var createRequest = new CreateEventRequest
        {
            EventType = "HighFive",
            Username = "user1",
            Recipient = "user2"
        };

        var newEvent = new HighFiveEvent
        {
            Id = Guid.NewGuid(),
            Username = "user1",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.HighFive,
            RecipientUsername = "user2"
        };

        var detailedResponse = new DetailedEventResponse
        {
            Id = newEvent.Id,
            Username = "user1",
            Timestamp = newEvent.Timestamp,
            EventType = "HighFive",
            RecipientUsername = "user2"
        };

        _mockEventFactory
            .Setup(factory => factory.CreateEvent(createRequest))
            .Returns(newEvent);

        _mockEventFactory
            .Setup(factory => factory.CreateDetailedChatEventResponse(newEvent))
            .Returns(detailedResponse);

        // Act
        var result = await ChatEventsHandler.CreateEvent(
            createRequest,
            _mockEventService.Object,
            _mockEventFactory.Object,
            CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRoute<DetailedEventResponse>>(result);
        Assert.Equal(detailedResponse, createdResult.Value);
        
        _mockEventService.Verify(service => 
            service.CreateEvent(It.Is<HighFiveEvent>(e => 
                e.RecipientUsername == "user2" && e.Username == "user1"), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}