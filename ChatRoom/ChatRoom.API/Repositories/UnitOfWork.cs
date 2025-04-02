using ChatRoom.API.Data;
using ChatRoom.API.Interfaces;

namespace ChatRoom.API.Repositories;

public class UnitOfWork(ChatRoomDbContext context) : IUnitOfWork
{
    public IChatEventRepository ChatEvents { get; } = new ChatEventRepository(context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}