using FBS.Domain.Common.Interfaces;

namespace FBS.Infrastructure.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IDomainEvent;
}