namespace FBS.Infrastructure.EventDispatcher;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(CancellationToken cancellationToken);
}