using Azure.Identity;
using Azure.Messaging.ServiceBus;
using FBS.Domain.Common.Interfaces;
using FBS.Infrastructure.Events.Mapping;
using FBS.Infrastructure.Events.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FBS.Infrastructure.Events;

public class ServiceBusEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusOptions _options;
    private readonly IEventMapper _eventMapper;
    private readonly ILogger<ServiceBusEventPublisher> _logger;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ServiceBusEventPublisher(
        IEventMapper eventMapper,
        IOptions<ServiceBusOptions> options,
        ILogger<ServiceBusEventPublisher> logger)
    {
        _options = options.Value;
        _eventMapper = eventMapper;
        _logger = logger;

        _client = new ServiceBusClient(_options.FullyQualifiedNamespace, new DefaultAzureCredential());
        _sender = _client.CreateSender(_options.QueueName);

        _logger.LogInformation("ServiceBusEventPublisher initialized for queue {QueueName} on {Namespace}", _options.QueueName, _options.FullyQualifiedNamespace);
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken) where TEvent : IExternalDomainEvent
    {
        try
        {
            var dto = _eventMapper.MapToDto(domainEvent);
            var serializedEvent = JsonSerializer.Serialize(dto, _jsonSerializerOptions);

            var messageBody = Encoding.UTF8.GetBytes(serializedEvent);
            var message = new ServiceBusMessage(messageBody)
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json",
                Subject = domainEvent.GetType().Name
            };

            message.ApplicationProperties.Add("EventType", domainEvent.GetType().FullName);
            message.ApplicationProperties.Add("PublishedAt", DateTimeOffset.UtcNow);
            message.ApplicationProperties.Add("Source", "FBS");

            await _sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation("Published event {EventType} to Service Bus. MessageId: {MessageId}", domainEvent.GetType().Name, message.MessageId);
        }
        catch (ServiceBusException ex) when (ex.IsTransient)
        {
            _logger.LogWarning(ex, "Transient error publishing event {EventType} to Service Bus. Will retry.", domainEvent.GetType().Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} to Service Bus queue {QueueName}", domainEvent.GetType().Name, _options.QueueName);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
