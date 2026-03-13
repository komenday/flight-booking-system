namespace FBS.Infrastructure.Events.Options;

public class EventPublisherOptions
{
    public const string SectionName = "EventPublisher";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public int TimeoutSeconds { get; set; }
}
