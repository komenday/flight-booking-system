using FBS.Application.Common.Interfaces;
using FBS.Domain.Common.Interfaces;
using MediatR;

namespace FBS.Infrastructure.Services;

public class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    private readonly IPublisher _publisher = publisher;

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _publisher.Publish(domainEvent, cancellationToken);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}