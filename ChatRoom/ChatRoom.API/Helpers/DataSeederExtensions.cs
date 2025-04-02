using ChatRoom.API.Data;

namespace ChatRoom.API.Helpers;

public static class DataSeederExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var serviceProvider = scope.ServiceProvider;
        try
        {
            DataSeeder.Initialize(serviceProvider);
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }

        return host;
    }
}