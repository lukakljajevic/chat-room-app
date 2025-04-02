using ChatRoom.API.Data;
using ChatRoom.API.Interfaces;
using ChatRoom.API.Repositories;
using ChatRoom.API.Services;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.API.Helpers;

public static class RegistrationExtensions
{
    public static void RegisterChatEventDependencies(this IServiceCollection services)
    {
        services.AddDbContext<ChatRoomDbContext>(options => options.UseInMemoryDatabase("ChatRoomDb"));
        
        services.AddScoped<IChatEventRepository, ChatEventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IChatEventFactory, ChatEventFactory>();
        services.AddScoped<IChatEventService, ChatEventService>();
    }
}