namespace FBS.Infrastructure.Events.Options;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string FullyQualifiedNamespace { get; set; } = string.Empty;

    public string QueueName { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;

    public int MaxRetryAttempts { get; set; } = 3;
}
