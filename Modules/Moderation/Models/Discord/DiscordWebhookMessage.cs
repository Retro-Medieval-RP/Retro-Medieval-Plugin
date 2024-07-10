using System.Collections.Generic;

namespace Moderation.Models.Discord;

internal class DiscordWebhookMessage
{
    // ReSharper disable once InconsistentNaming
    public List<Embed> embeds { get; set; }
    // ReSharper disable once InconsistentNaming
    public string content { get; set; }
    
    public DiscordWebhookMessage(Embed embed)
    {
        embeds = [embed];
    }

    public DiscordWebhookMessage(string content)
    {
        this.content = content;
    }
}