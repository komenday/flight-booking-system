namespace FBS.Function.Notifications.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlContent, CancellationToken cancellationToken);
}