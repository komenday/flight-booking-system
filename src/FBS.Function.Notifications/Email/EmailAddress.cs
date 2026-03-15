using System.Text.Json.Serialization;

namespace FBS.Function.Notification.Email;

internal record EmailAddress
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}