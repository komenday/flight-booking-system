using FBS.Function.Notification.Email;
using FBS.Function.Notifications.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace FBS.Function.Notifications.Email;

public class MailtrapApiEmailService(
    HttpClient httpClient,
    IOptions<MailtrapApiOptions> options,
    ILogger<MailtrapApiEmailService> logger
    //IFileNotificationLogger fileNotificationLogger
    ) : IEmailService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly MailtrapApiOptions _options = options.Value;
    private readonly ILogger<MailtrapApiEmailService> _logger = logger;
    //private readonly IFileNotificationLogger _fileNotificationLogger = fileNotificationLogger;

    public async Task SendAsync(
        string to,
        string subject,
        string htmlContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending email via Mailtrap API to {Email} with subject '{Subject}'", to, subject);

            var request = new MailtrapEmailRequest
            {
                From = new EmailAddress
                {
                    Email = _options.FromEmail,
                    Name = _options.FromName
                },
                To =
                [
                    new EmailAddress
                    {
                        Email = to
                    }
                ],
                Subject = subject,
                Html = htmlContent
            };

            _logger.LogDebug("Sending POST to Mailtrap API: {Endpoint}", $"api/send/{_options.InboxId}");

            var response = await _httpClient.PostAsJsonAsync(
                $"api/send/{_options.InboxId}",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MailtrapApiResponse>(cancellationToken);

                _logger.LogInformation("✅ Email sent successfully via Mailtrap API to {Email}. " + "Message IDs: {MessageIds}. " + "Check inbox at: https://mailtrap.io/inboxes/{InboxId}",
                    to, string.Join(", ", result?.MessageIds ?? []), _options.InboxId);

                //await _fileNotificationLogger.LogNotificationSentAsync(new NotificationLog
                //{
                //    To = to,
                //    Subject = subject,
                //    Provider = "Mailtrap API",
                //    SentAt = DateTime.UtcNow
                //});
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "❌ Mailtrap API returned error. Status: {StatusCode}, Body: {Error}", response.StatusCode, errorContent);

                //await _fileNotificationLogger.LogNotificationFailedAsync(
                //    to,
                //    subject,
                //    $"HTTP {response.StatusCode}: {errorContent}");

                throw new HttpRequestException(
                    $"Mailtrap API error: {response.StatusCode} - {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ HTTP error while sending email via Mailtrap API to {Email}", to);

            //await _fileNotificationLogger.LogNotificationFailedAsync(
            //    to,
            //    subject,
            //    $"HTTP error: {ex.Message}");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error while sending email via Mailtrap API to {Email}", to);

            //await _fileNotificationLogger.LogNotificationFailedAsync(to, subject, ex.Message);

            throw;
        }
    }
}