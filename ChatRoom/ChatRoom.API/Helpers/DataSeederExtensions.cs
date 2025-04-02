using ChatRoom.API.Data;

namespace ChatRoom.API.Helpers;

public static class DataSeederExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        try
        {
            DataSeeder.Initialize(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }

        return host;
    }
}