namespace ChatRoom.API.Interfaces;

public interface IUnitOfWork
{
    IChatEventRepository ChatEvents { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}