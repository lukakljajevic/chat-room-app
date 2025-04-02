using AutoMapper;
using ChatRoom.API.Common;
using ChatRoom.API.DTO;
using ChatRoom.API.Entities;
using ChatRoom.API.Services;
using Moq;

namespace ChatRoom.Tests;

public class ChatEventFactoryTests
{
    private readonly ChatEventFactory _factory;

    public ChatEventFactoryTests()
    {
        var mockMapper = new Mock<IMapper>();
        var mapper = mockMapper.Object;
        _factory = new ChatEventFactory(mapper);
        
        SetupMapperMocks(mockMapper);
    }

    private static void SetupMapperMocks(Mock<IMapper> mockMapper)
    {
        // Setup for CreateEvent method
        mockMapper.Setup(m => m.Map<EnterRoomEvent>(It.IsAny<CreateEventRequest>()))
            .Returns((CreateEventRequest req) => new EnterRoomEvent { 
                Id = Guid.NewGuid(), 
                Username = req.Username, 
                Timestamp = DateTime.UtcNow,
                EventType = EventType.EnterRoom 
            });

        mockMapper.Setup(m => m.Map<LeaveRoomEvent>(It.IsAny<CreateEventRequest>()))
            .Returns((CreateEventRequest req) => new LeaveRoomEvent { 
                Id = Guid.NewGuid(), 
                Username = req.Username, 
                Timestamp = DateTime.UtcNow,
                EventType = EventType.LeaveRoom 
            });

        mockMapper.Setup(m => m.Map<CommentEvent>(It.IsAny<CreateEventRequest>()))
            .Returns((CreateEventRequest req) => new CommentEvent { 
                Id = Guid.NewGuid(), 
                Username = req.Username, 
                Timestamp = DateTime.UtcNow,
                EventType = EventType.Comment,
                CommentText = req.CommentText! 
            });

        mockMapper.Setup(m => m.Map<HighFiveEvent>(It.IsAny<CreateEventRequest>()))
            .Returns((CreateEventRequest req) => new HighFiveEvent { 
                Id = Guid.NewGuid(), 
                Username = req.Username, 
                Timestamp = DateTime.UtcNow,
                EventType = EventType.HighFive,
                RecipientUsername = req.Recipient! 
            });

        // Setup for responses
        mockMapper.Setup(m => m.Map<DetailedEventResponse>(It.IsAny<ChatEvent>()))
            .Returns((ChatEvent e) => 
            {
                var response = new DetailedEventResponse
                {
                    Id = e.Id,
                    Username = e.Username,
                    Timestamp = e.Timestamp,
                    EventType = e.EventType.ToString()
                };

                switch (e)
                {
                    case CommentEvent commentEvent:
                        response.CommentText = commentEvent.CommentText;
                        break;
                    case HighFiveEvent highFiveEvent:
                        response.RecipientUsername = highFiveEvent.RecipientUsername;
                        break;
                }

                return response;
            });

        mockMapper.Setup(m => m.Map<IEnumerable<DetailedEventResponse>>(It.IsAny<IEnumerable<ChatEvent>>()))
            .Returns((IEnumerable<ChatEvent> events) => 
                events.Select(e => 
                {
                    var response = new DetailedEventResponse
                    {
                        Id = e.Id,
                        Username = e.Username,
                        Timestamp = e.Timestamp,
                        EventType = e.EventType.ToString()
                    };

                    switch (e)
                    {
                        case CommentEvent commentEvent:
                            response.CommentText = commentEvent.CommentText;
                            break;
                        case HighFiveEvent highFiveEvent:
                            response.RecipientUsername = highFiveEvent.RecipientUsername;
                            break;
                    }

                    return response;
                })
            );
    }

    [Fact]
    public void CreateEvent_WithEnterRoomType_ReturnsEnterRoomEvent()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "EnterRoom",
            Username = "testuser"
        };

        // Act
        var result = _factory.CreateEvent(request);

        // Assert
        Assert.IsType<EnterRoomEvent>(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(EventType.EnterRoom, result.EventType);
    }

    [Fact]
    public void CreateEvent_WithLeaveRoomType_ReturnsLeaveRoomEvent()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "LeaveRoom",
            Username = "testuser"
        };

        // Act
        var result = _factory.CreateEvent(request);

        // Assert
        Assert.IsType<LeaveRoomEvent>(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(EventType.LeaveRoom, result.EventType);
    }

    [Fact]
    public void CreateEvent_WithCommentType_ReturnsCommentEvent()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "Comment",
            Username = "testuser",
            CommentText = "Hello world"
        };

        // Act
        var result = _factory.CreateEvent(request);

        // Assert
        var commentEvent = Assert.IsType<CommentEvent>(result);
        Assert.Equal("testuser", commentEvent.Username);
        Assert.Equal(EventType.Comment, commentEvent.EventType);
        Assert.Equal("Hello world", commentEvent.CommentText);
    }

    [Fact]
    public void CreateEvent_WithHighFiveType_ReturnsHighFiveEvent()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "HighFive",
            Username = "testuser",
            Recipient = "otheruser"
        };

        // Act
        var result = _factory.CreateEvent(request);

        // Assert
        var highFiveEvent = Assert.IsType<HighFiveEvent>(result);
        Assert.Equal("testuser", highFiveEvent.Username);
        Assert.Equal(EventType.HighFive, highFiveEvent.EventType);
        Assert.Equal("otheruser", highFiveEvent.RecipientUsername);
    }

    [Fact]
    public void CreateEvent_WithMixedCaseType_WorksCorrectly()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "cOmMeNt",
            Username = "testuser",
            CommentText = "Test comment"
        };

        // Act
        var result = _factory.CreateEvent(request);

        // Assert
        Assert.IsType<CommentEvent>(result);
    }

    [Fact]
    public void CreateEvent_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            EventType = "InvalidType",
            Username = "testuser"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateEvent(request));
        Assert.Contains("Unknown event type", exception.Message);
    }

    [Fact]
    public void CreateDetailedChatEventResponse_MapsCorrectly()
    {
        // Arrange
        var chatEvent = new CommentEvent
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Timestamp = DateTime.UtcNow,
            EventType = EventType.Comment,
            CommentText = "Test comment"
        };

        // Act
        var result = _factory.CreateDetailedChatEventResponse(chatEvent);

        // Assert
        Assert.Equal(chatEvent.Id, result.Id);
        Assert.Equal(chatEvent.Username, result.Username);
        Assert.Equal(chatEvent.Timestamp, result.Timestamp);
        Assert.Equal("Comment", result.EventType);
        Assert.Equal("Test comment", result.CommentText);
    }

    [Fact]
    public void CreateChatEventResponses_MapsMultipleEvents()
    {
        // Arrange
        var events = new List<ChatEvent>
        {
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                EventType = EventType.EnterRoom
            },
            new CommentEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = DateTime.UtcNow,
                EventType = EventType.Comment,
                CommentText = "Hello"
            }
        };

        // Act
        var results = _factory.CreateChatEventResponses(events).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(events[0].Id, results[0].Id);
        Assert.Equal(events[1].Id, results[1].Id);
        Assert.Equal("EnterRoom", results[0].EventType);
        Assert.Equal("Comment", results[1].EventType);
        Assert.Equal("Hello", results[1].CommentText);
    }

    [Fact]
    public void CreateAggregatedChatEventResponses_GroupsByHour()
    {
        // Arrange
        var baseTime = new DateTime(2025, 4, 1, 10, 0, 0);
        
        var events = new List<ChatEvent>
        {
            // First hour: 10:00
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = baseTime.AddMinutes(10),
                EventType = EventType.EnterRoom
            },
            new CommentEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = baseTime.AddMinutes(20),
                EventType = EventType.Comment,
                CommentText = "Hello"
            },
            
            // Second hour: 11:00
            new HighFiveEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = baseTime.AddHours(1).AddMinutes(5),
                EventType = EventType.HighFive,
                RecipientUsername = "user2"
            },
            new LeaveRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user2",
                Timestamp = baseTime.AddHours(1).AddMinutes(30),
                EventType = EventType.LeaveRoom
            }
        };

        // Act
        var results = _factory.CreateAggregatedChatEventResponses(events, GranularityLevel.Hour).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        
        // Verify the results are ordered by descending timestamp
        Assert.Equal(baseTime.AddHours(1), results[0].PeriodStart);
        Assert.Equal(baseTime, results[1].PeriodStart);
        
        // Verify counts for the 11:00 hour
        Assert.Equal(0, results[0].EntersCount);
        Assert.Equal(1, results[0].LeavesCount);
        Assert.Equal(0, results[0].CommentsCount);
        Assert.Equal(1, results[0].HighFivesCount);
        Assert.Single(results[0].HighFiveDetails);
        Assert.Contains("user1 high-fived user2", results[0].HighFiveDetails);
        
        // Verify counts for the 10:00 hour
        Assert.Equal(1, results[1].EntersCount);
        Assert.Equal(0, results[1].LeavesCount);
        Assert.Equal(1, results[1].CommentsCount);
        Assert.Equal(0, results[1].HighFivesCount);
        Assert.Empty(results[1].HighFiveDetails);
    }

    [Theory]
    [InlineData(GranularityLevel.Minute, 2025, 4, 1, 10, 15, 30, 2025, 4, 1, 10, 15, 0)]
    [InlineData(GranularityLevel.Hour, 2025, 4, 1, 10, 15, 30, 2025, 4, 1, 10, 0, 0)]
    [InlineData(GranularityLevel.Day, 2025, 4, 1, 10, 15, 30, 2025, 4, 1, 0, 0, 0)]
    [InlineData(GranularityLevel.Month, 2025, 4, 15, 10, 15, 30, 2025, 4, 1, 0, 0, 0)]
    public void CreateAggregatedChatEventResponses_TruncatesToCorrectGranularity(
        GranularityLevel granularity, 
        int year, int month, int day, int hour, int minute, int second,
        int expectedYear, int expectedMonth, int expectedDay, 
        int expectedHour, int expectedMinute, int expectedSecond)
    {
        // Arrange
        var timestamp = new DateTime(year, month, day, hour, minute, second);
        var expectedTimestamp = new DateTime(expectedYear, expectedMonth, expectedDay, 
                                             expectedHour, expectedMinute, expectedSecond);
        
        var events = new List<ChatEvent>
        {
            new EnterRoomEvent
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Timestamp = timestamp,
                EventType = EventType.EnterRoom
            }
        };

        // Act
        var results = _factory.CreateAggregatedChatEventResponses(events, granularity).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(expectedTimestamp, results[0].PeriodStart);
    }

    [Fact]
    public void CreateAggregatedChatEventResponses_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var events = new List<ChatEvent>();

        // Act
        var results = _factory.CreateAggregatedChatEventResponses(events, GranularityLevel.Hour);

        // Assert
        Assert.Empty(results);
    }
}