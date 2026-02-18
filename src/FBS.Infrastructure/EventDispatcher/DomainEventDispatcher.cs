using FBS.Domain.Common.Interfaces;
using FBS.Infrastructure.Events;
using FBS.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Infrastructure.EventDispatcher;

public class DomainEventDispatcher(
    ApplicationDbContext context,
    IPublisher mediator,
    IEventPublisher eventPublisher,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    private readonly ApplicationDbContext _context = context;
    private readonly IPublisher _mediator = mediator;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<DomainEventDispatcher> _logger = logger;

    public async Task DispatchEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAggregateRoot aggregate && aggregate.DomainEvents.Count != 0)
            .Select(e => (IAggregateRoot)e.Entity)
            .ToList();

        if (domainEntities.Count == 0)
        {
            return;
        }

        var domainEvents = domainEntities
            .SelectMany(agg => agg.DomainEvents)
            .ToList();

        _logger.LogInformation("Dispatching {Count} domain events", domainEvents.Count);

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        var externalEvents = domainEvents.OfType<IExternalDomainEvent>().ToList();
        var internalEvents = domainEvents.Except(externalEvents).ToList();

        #region Internal events - currently no handlers, but infrastructure ready
        if (internalEvents.Count != 0)
        {
            _logger.LogDebug("Publishing {Count} events locally (parallel)", internalEvents.Count);

            var localPublishTasks = internalEvents.Select(async domainEvent =>
            {
                try
                {
                    _logger.LogDebug("Publishing event locally: {EventType}", domainEvent.GetType().Name);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing event {EventType} locally", domainEvent.GetType().Name);
                }
            });

            await Task.WhenAll(localPublishTasks);
        }
        #endregion

        if (externalEvents.Count != 0)
        {
            _logger.LogDebug("Publishing {Count} events externally (fire-and-forget)", externalEvents.Count);

            var externalPublishTasks = externalEvents.Select(domainEvent =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogDebug("Publishing event externally: {EventType}", domainEvent.GetType().Name);
                        await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to publish event {EventType} externally", domainEvent.GetType().Name);
                    }
                }, cancellationToken);

                return Task.CompletedTask;
            });

            _ = Task.WhenAll(externalPublishTasks);
        }

        _logger.LogInformation("Domain events dispatched successfully");
    }
}