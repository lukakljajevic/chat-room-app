using ChatRoom.API.Common;
using ChatRoom.API.Data;
using ChatRoom.API.Entities;
using ChatRoom.API.Interfaces;
using ChatRoom.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Tests;

public class UnitOfWorkTests
{
    [Fact]
    public void UnitOfWork_Initializes_ChatEventsRepository()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ChatRoomDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        using var context = new ChatRoomDbContext(options);
        
        // Act
        var unitOfWork = new UnitOfWork(context);
        
        // Assert
        Assert.NotNull(unitOfWork.ChatEvents);
        Assert.IsAssignableFrom<IChatEventRepository>(unitOfWork.ChatEvents);
    }
    
    [Fact]
    public async Task CommitAsync_SavesChangesToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ChatRoomDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        
        await using var context = new ChatRoomDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        
        var newEvent = new CommentEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Username = "testuser",
            EventType = EventType.Comment,
            CommentText = "Test comment"
        };
        
        unitOfWork.ChatEvents.AddEvent(newEvent);
        
        // Act
        var saveResult = await unitOfWork.CommitAsync();
        
        // Assert
        Assert.Equal(1, saveResult);
        
        // Verify entity was saved by retrieving with a new context
        await using var verifyContext = new ChatRoomDbContext(options);
        var savedEvent = await verifyContext.Events.FindAsync(newEvent.Id);
        
        Assert.NotNull(savedEvent);
        Assert.Equal(newEvent.Id, savedEvent.Id);
        Assert.Equal(newEvent.Username, savedEvent.Username);
        Assert.Equal(EventType.Comment, savedEvent.EventType);
        
        var commentEvent = Assert.IsType<CommentEvent>(savedEvent);
        Assert.Equal(newEvent.CommentText, commentEvent.CommentText);
    }
    
    [Fact]
    public async Task CommitAsync_PreservesEventTypeAndFields()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ChatRoomDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        await using var context = new ChatRoomDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        
        var enterEvent = new EnterRoomEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow.AddMinutes(-10),
            Username = "user1",
            EventType = EventType.EnterRoom
        };
        
        var commentEvent = new CommentEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow.AddMinutes(-5),
            Username = "user1",
            EventType = EventType.Comment,
            CommentText = "Hello everyone!"
        };
        
        var highFiveEvent = new HighFiveEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Username = "user1",
            EventType = EventType.HighFive,
            RecipientUsername = "user2"
        };
        
        unitOfWork.ChatEvents.AddEvent(enterEvent);
        unitOfWork.ChatEvents.AddEvent(commentEvent);
        unitOfWork.ChatEvents.AddEvent(highFiveEvent);
        
        // Act
        await unitOfWork.CommitAsync();
        
        // Assert
        await using var verifyContext = new ChatRoomDbContext(options);
        
        var savedEnterEvent = await verifyContext.Events.FindAsync(enterEvent.Id);
        Assert.NotNull(savedEnterEvent);
        Assert.IsType<EnterRoomEvent>(savedEnterEvent);
        Assert.Equal(EventType.EnterRoom, savedEnterEvent.EventType);
        
        var savedCommentEvent = await verifyContext.Events.FindAsync(commentEvent.Id);
        Assert.NotNull(savedCommentEvent);
        var typedCommentEvent = Assert.IsType<CommentEvent>(savedCommentEvent);
        Assert.Equal(EventType.Comment, savedCommentEvent.EventType);
        Assert.Equal(commentEvent.CommentText, typedCommentEvent.CommentText);
        
        var savedHighFiveEvent = await verifyContext.Events.FindAsync(highFiveEvent.Id);
        Assert.NotNull(savedHighFiveEvent);
        var typedHighFiveEvent = Assert.IsType<HighFiveEvent>(savedHighFiveEvent);
        Assert.Equal(EventType.HighFive, savedHighFiveEvent.EventType);
        Assert.Equal(highFiveEvent.RecipientUsername, typedHighFiveEvent.RecipientUsername);
    }
}