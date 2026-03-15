using System.Text.Json.Serialization;

namespace FBS.Function.Notification.Email;

internal record MailtrapApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message_ids")]
    public string[] MessageIds { get; init; } = [];
}