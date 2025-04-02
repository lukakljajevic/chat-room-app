using ChatRoom.API.Common;
using ChatRoom.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.API.Data;

public class ChatRoomDbContext : DbContext
{
    public ChatRoomDbContext(DbContextOptions<ChatRoomDbContext> options) : base(options)
    {
        
    }
    
    public ChatRoomDbContext()
    {
        
    }
    
    public virtual DbSet<ChatEvent> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatEvent>()
            .HasDiscriminator(e => e.EventType)
            .HasValue<EnterRoomEvent>(EventType.EnterRoom)
            .HasValue<LeaveRoomEvent>(EventType.LeaveRoom)
            .HasValue<CommentEvent>(EventType.Comment)
            .HasValue<HighFiveEvent>(EventType.HighFive);

        modelBuilder.Entity<ChatEvent>()
            .Property(e => e.Username)
            .HasMaxLength(100)
            .IsRequired();
        
        modelBuilder.Entity<CommentEvent>()
            .Property(e => e.CommentText)
            .IsRequired();
                
        modelBuilder.Entity<HighFiveEvent>()
            .Property(e => e.RecipientUsername)
            .IsRequired();
    }
}