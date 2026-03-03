using FBS.Domain.Common.Interfaces;
using FBS.Domain.Reservation.Events;
using FBS.Infrastructure.Events.Mapping;
using Microsoft.Extensions.Logging;

namespace FBS.Infrastructure.Events;

public class HttpEventPublisher(
    NotificationSystemHttpClient fbnsClient,
    ILogger<HttpEventPublisher> logger) : IEventPublisher
{
    private readonly NotificationSystemHttpClient _fbnsClient = fbnsClient;
    private readonly ILogger<HttpEventPublisher> _logger = logger;

    public async Task PublishAsync<TEvent>(TEvent @event,CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        var (endpoint, dto) = MapEventToEndpointAndDto(@event);

        if (endpoint == null || dto == null)
        {
            _logger.LogWarning("No endpoint or DTO mapping for event type {EventType}. Skipping publication.", typeof(TEvent).Name);
            return;
        }

        try
        {
            var response = await _fbnsClient.PostEventAsync(endpoint, dto, cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Event {EventType} successfully published to FBNS", typeof(TEvent).Name);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} to FBNS after retries: {Message}", typeof(TEvent).Name, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout publishing event {EventType} to FBNS", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing event {EventType} to FBNS", typeof(TEvent).Name);
        }
    }

    private static (string? endpoint, object? dto) MapEventToEndpointAndDto<TEvent>(TEvent @event)
        where TEvent : IDomainEvent
    {
        return @event switch
        {
            ReservationCreatedEvent created => ("/webhooks/reservation-created", created.ToDto()),
            ReservationConfirmedEvent confirmed => ("/webhooks/reservation-confirmed", confirmed.ToDto()),
            ReservationCancelledEvent cancelled => ("/webhooks/reservation-cancelled", cancelled.ToDto()),
            ReservationExpiredEvent expired => ("/webhooks/reservation-expired", expired.ToDto()),
            _ => (null, null)
        };
    }
}