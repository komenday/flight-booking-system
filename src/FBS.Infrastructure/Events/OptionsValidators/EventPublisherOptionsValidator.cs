using FBS.Infrastructure.Events.Options;
using Microsoft.Extensions.Options;

namespace FBS.Infrastructure.Events.OptionsValidators;

public class EventPublisherOptionsValidator : IValidateOptions<EventPublisherOptions>
{
    public ValidateOptionsResult Validate(string? name, EventPublisherOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return ValidateOptionsResult.Fail(
                "EventPublisher:BaseUrl is required. " +
                "Please configure it in appsettings.json under 'EventPublisher' section.");
        }

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
        {
            return ValidateOptionsResult.Fail(
                $"EventPublisher:BaseUrl '{options.BaseUrl}' is not a valid URL.");
        }

        if (options.RetryCount < 0 || options.RetryCount > 10)
        {
            return ValidateOptionsResult.Fail(
                "EventPublisher:RetryCount must be between 0 and 10.");
        }

        if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
        {
            return ValidateOptionsResult.Fail(
                "EventPublisher:TimeoutSeconds must be between 1 and 300.");
        }

        return ValidateOptionsResult.Success;
    }
}