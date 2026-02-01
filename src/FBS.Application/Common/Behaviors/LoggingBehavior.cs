using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FBS.Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms. Success: {IsSuccess}", requestName, stopwatch.ElapsedMilliseconds, IsSuccessResult(response));

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "Error handling {RequestName} in {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    private static bool IsSuccessResult(TResponse response)
    {
        if (response is null)
            return false;

        var type = response.GetType();

        var isSuccessProperty = type.GetProperty("IsSuccess");
        if (isSuccessProperty is not null)
        {
            return (bool)(isSuccessProperty.GetValue(response) ?? false);
        }

        return true;
    }
}
