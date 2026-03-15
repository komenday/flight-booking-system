namespace FBS.Function.Notification.Email;

public class MailtrapApiOptions
{
    public const string SectionName = "MailtrapApi";

    public string ApiToken { get; set; } = string.Empty;

    public string InboxId { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;
}