using Newtonsoft.Json;

namespace Moderation.Models.Discord;

internal class DiscordWebhook
{
    public DiscordWebhook() { }

    public DiscordWebhook(string webhookType, string webhookUrl, string webhookColor, string messageFormat, bool inline = false)
    {
        WebhookType = webhookType;
        WebhookUrl = webhookUrl;
        WebhookColor = webhookColor;
        MessageFormat = messageFormat;
        Inline = inline;
    }

    public string WebhookType { get; set; }
    public string WebhookUrl { get; set; }
    public string WebhookColor { get; set; }
    public bool Inline { get; set; }
    public string MessageFormat { get; set; }
    
    [JsonIgnore]
    public string Title { get; set; }
    [JsonIgnore]
    public string Url { get; set; }
}