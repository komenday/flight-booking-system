using Azure.Messaging.ServiceBus;
using FBS.Function.Notifications.Interfaces;
using FBS.Infrastructure.Events.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FBS.Function.Notifications.Functions;

public class ProcessReservationEventFunction(
    INotificationService notificationService,
    ILogger<ProcessReservationEventFunction> logger)
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<ProcessReservationEventFunction> _logger = logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Function(nameof(ProcessReservationEvent))]
    public async Task ProcessReservationEvent(
        [ServiceBusTrigger(queueName: "reservation-events", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        var eventType = message.Subject;

        _logger.LogInformation("Processing message {MessageId}, Subject: {Subject}", message.MessageId, message.Subject);

        if (!IsKnownEventType(eventType))
        {
            _logger.LogError("Unknown event type '{EventType}' in message {MessageId}. Sending to Dead-Letter Queue.", eventType, message.MessageId);

            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "UnknownEventType",
                deadLetterErrorDescription: $"Event type '{eventType}' is not supported.",
                cancellationToken: cancellationToken);

            return;
        }

        try
        {
            var messageBody = message.Body.ToString();

            await (eventType switch
            {
                "ReservationCreatedEvent" => ProcessCreatedAsync(messageBody, cancellationToken),
                "ReservationConfirmedEvent" => ProcessConfirmedAsync(messageBody, cancellationToken),
                "ReservationCancelledEvent" => ProcessCancelledAsync(messageBody, cancellationToken),
                "ReservationExpiredEvent" => ProcessExpiredAsync(messageBody, cancellationToken),
                _ => Task.CompletedTask
            });

            await messageActions.CompleteMessageAsync(message, cancellationToken);

            _logger.LogInformation("Message {MessageId} processed successfully", message.MessageId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message {MessageId} (event: {EventType}). Sending to Dead-Letter Queue.", message.MessageId, eventType);

            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "DeserializationFailed",
                deadLetterErrorDescription: ex.Message,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transient error processing message {MessageId} (event: {EventType}). Abandoning for retry.", message.MessageId, eventType);

            await messageActions.AbandonMessageAsync(message, cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessCreatedAsync(string body, CancellationToken cancellationToken)
    {
        var dto = JsonSerializer.Deserialize<ReservationCreatedEventDto>(body, _jsonOptions)
            ?? throw new JsonException("Failed to deserialize ReservationCreatedEventDto: result was null");
        await _notificationService.HandleReservationCreatedAsync(dto, cancellationToken);
    }

    private async Task ProcessConfirmedAsync(string body, CancellationToken cancellationToken)
    {
        var dto = JsonSerializer.Deserialize<ReservationConfirmedEventDto>(body, _jsonOptions)
            ?? throw new JsonException("Failed to deserialize ReservationConfirmedEventDto: result was null");

        await _notificationService.HandleReservationConfirmedAsync(dto, cancellationToken);
    }

    private async Task ProcessCancelledAsync(string body, CancellationToken cancellationToken)
    {
        var dto = JsonSerializer.Deserialize<ReservationCancelledEventDto>(body, _jsonOptions)
            ?? throw new JsonException("Failed to deserialize ReservationCancelledEventDto: result was null");

        await _notificationService.HandleReservationCancelledAsync(dto, cancellationToken);
    }

    private async Task ProcessExpiredAsync(string body, CancellationToken cancellationToken)
    {
        var dto = JsonSerializer.Deserialize<ReservationExpiredEventDto>(body, _jsonOptions)
            ?? throw new JsonException("Failed to deserialize ReservationExpiredEventDto: result was null");

        await _notificationService.HandleReservationExpiredAsync(dto, cancellationToken);
    }

    private static bool IsKnownEventType(string? eventType) => eventType switch
    {
        "ReservationCreatedEvent" => true,
        "ReservationConfirmedEvent" => true,
        "ReservationCancelledEvent" => true,
        "ReservationExpiredEvent" => true,
        _ => false
    };
}
