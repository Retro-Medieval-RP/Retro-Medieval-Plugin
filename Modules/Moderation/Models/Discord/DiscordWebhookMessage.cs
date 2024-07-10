using System.Collections.Generic;

namespace Moderation.Models.Discord;

internal class DiscordWebhookMessage
{
    public List<Embed> Embeds { get; set; }
    public string Content { get; set; }
    
    public DiscordWebhookMessage(Embed embed)
    {
        Embeds = [embed];
    }

    public DiscordWebhookMessage(string content)
    {
        Content = content;
    }
}