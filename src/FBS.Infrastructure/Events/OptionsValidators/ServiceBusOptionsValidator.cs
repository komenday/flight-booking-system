using FBS.Infrastructure.Events.Options;
using Microsoft.Extensions.Options;

namespace FBS.Infrastructure.Events.OptionsValidators;

public class ServiceBusOptionsValidator : IValidateOptions<ServiceBusOptions>
{
    public ValidateOptionsResult Validate(string? name, ServiceBusOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace))
        {
            errors.Add($"{nameof(options.FullyQualifiedNamespace)} is required.");
        }
        else if (options.FullyQualifiedNamespace.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 options.FullyQualifiedNamespace.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add($"{nameof(options.FullyQualifiedNamespace)} should not include http:// or https:// prefix.");
        }
        else if (!options.FullyQualifiedNamespace.EndsWith(".servicebus.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add($"{nameof(options.FullyQualifiedNamespace)} should end with '.servicebus.windows.net'.");
        }

        if (string.IsNullOrWhiteSpace(options.QueueName))
        {
            errors.Add($"{nameof(options.QueueName)} is required.");
        }

        if (options.TimeoutSeconds <= 0)
        {
            errors.Add($"{nameof(options.TimeoutSeconds)} must be greater than 0.");
        }

        if (options.MaxRetryAttempts < 0)
        {
            errors.Add($"{nameof(options.MaxRetryAttempts)} must be 0 or greater.");
        }

        if (errors.Count != 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}