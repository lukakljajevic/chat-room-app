using ChatRoom.API.Common;
using ChatRoom.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.API.Data;

public static class DataSeeder
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ChatRoomDbContext(serviceProvider.GetRequiredService<DbContextOptions<ChatRoomDbContext>>());
        SeedData(context);
    }

    private static void SeedData(ChatRoomDbContext context)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var yesterday = today.AddDays(-1);
        var twoDaysAgo = today.AddDays(-2);
        var lastWeek = today.AddDays(-7);
        
        context.Events.AddRange(
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(0), 
                Username = "Bob",
                EventType = EventType.EnterRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(5), 
                Username = "Kate",
                EventType = EventType.EnterRoom
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(15), 
                Username = "Bob", 
                EventType = EventType.Comment,
                CommentText = "Hey, Kate - high five?"
            },
            new HighFiveEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(17), 
                Username = "Kate", 
                EventType = EventType.HighFive,
                RecipientUsername = "Bob"
            },
            new LeaveRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(18), 
                Username = "Bob",
                EventType = EventType.LeaveRoom
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(20), 
                Username = "Kate", 
                EventType = EventType.Comment,
                CommentText = "Oh, typical"
            },
            new LeaveRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(9).AddMinutes(21), 
                Username = "Kate",
                EventType = EventType.LeaveRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(0), 
                Username = "Alice",
                EventType = EventType.EnterRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(5), 
                Username = "Dave",
                EventType = EventType.EnterRoom
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(10), 
                Username = "Alice", 
                EventType = EventType.Comment,
                CommentText = "Hey Dave, how's the project going?"
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(12), 
                Username = "Dave", 
                EventType = EventType.Comment,
                CommentText = "Making progress! Want to see the demo later?"
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(15), 
                Username = "Alice", 
                EventType = EventType.Comment,
                CommentText = "Absolutely!"
            },
            new HighFiveEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = today.AddHours(14).AddMinutes(16), 
                Username = "Alice", 
                EventType = EventType.HighFive,
                RecipientUsername = "Dave"
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = yesterday.AddHours(10).AddMinutes(0), 
                Username = "Charlie",
                EventType = EventType.EnterRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = yesterday.AddHours(10).AddMinutes(10), 
                Username = "Eve",
                EventType = EventType.EnterRoom
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = yesterday.AddHours(10).AddMinutes(15), 
                Username = "Charlie", 
                EventType = EventType.Comment,
                CommentText = "Morning team meeting in 15 minutes."
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = yesterday.AddHours(10).AddMinutes(20), 
                Username = "Eve", 
                EventType = EventType.Comment,
                CommentText = "I'll be there!"
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = twoDaysAgo.AddHours(11).AddMinutes(0), 
                Username = "Frank",
                EventType = EventType.EnterRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = twoDaysAgo.AddHours(11).AddMinutes(5), 
                Username = "Grace",
                EventType = EventType.EnterRoom
            },
            new HighFiveEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = twoDaysAgo.AddHours(11).AddMinutes(10), 
                Username = "Frank", 
                EventType = EventType.HighFive,
                RecipientUsername = "Grace"
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = twoDaysAgo.AddHours(11).AddMinutes(15), 
                Username = "Grace", 
                EventType = EventType.Comment,
                CommentText = "Thanks for helping with that bug fix yesterday!"
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(0), 
                Username = "Heidi",
                EventType = EventType.EnterRoom
            },
            new EnterRoomEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(5), 
                Username = "Ivan",
                EventType = EventType.EnterRoom
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(10), 
                Username = "Heidi", 
                EventType = EventType.Comment,
                CommentText = "Let's plan the sprint for next week."
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(15), 
                Username = "Ivan", 
                EventType = EventType.Comment,
                CommentText = "I think we should focus on the new feature first."
            },
            new HighFiveEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(20), 
                Username = "Heidi", 
                EventType = EventType.HighFive,
                RecipientUsername = "Ivan"
            },
            new CommentEvent 
            { 
                Id = Guid.NewGuid(), 
                Timestamp = lastWeek.AddHours(13).AddMinutes(25), 
                Username = "Heidi", 
                EventType = EventType.Comment,
                CommentText = "Great idea!"
            }
        );
        
        context.SaveChanges();
    }
}