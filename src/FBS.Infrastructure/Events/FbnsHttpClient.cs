using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FBS.Infrastructure.Events;

public class FbnsHttpClient(HttpClient httpClient, ILogger<FbnsHttpClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<FbnsHttpClient> _logger = logger;

    public async Task<HttpResponseMessage> PostEventAsync<TEvent>(string endpoint, TEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Sending POST request to FBNS: {Endpoint}", endpoint);

            var response = await _httpClient.PostAsJsonAsync(endpoint, @event, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully published event to FBNS: {EventType} → {Endpoint}", typeof(TEvent).Name, endpoint);
            }
            else
            {
                _logger.LogWarning("FBNS returned non-success status: {StatusCode} for {EventType}", response.StatusCode, typeof(TEvent).Name);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when publishing {EventType} to FBNS: {Message}", typeof(TEvent).Name, ex.Message);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout when publishing {EventType} to FBNS", typeof(TEvent).Name);
            throw;
        }
    }
}
