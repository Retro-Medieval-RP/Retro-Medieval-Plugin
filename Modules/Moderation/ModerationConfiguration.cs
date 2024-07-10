using System.Collections.Generic;
using Moderation.Models;
using Moderation.Models.Discord;
using RetroMedieval.Modules.Configuration;

namespace Moderation;

internal class ModerationConfiguration : IConfig
{
    public bool UseWebhooks { get; set; }
    public bool ServerFooterEnabled { get; set; }
    public List<DiscordWebhook> Webhooks { get; set; }
    
    public void LoadDefaults()
    {
        UseWebhooks = true;
        ServerFooterEnabled = true;
        Webhooks =
        [
            new DiscordWebhook("Ban", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"),
            new DiscordWebhook("Unban", "", "", "Name: {name}, SteamID: {steamid}, Unbanner: {punisher}"),
            new DiscordWebhook("Mute", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"),
            new DiscordWebhook("Unmute", "", "", "Name: {name}, SteamID: {steamid}, Unmuter: {punisher}"),
            new DiscordWebhook("Kick", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"),
            new DiscordWebhook("Warn", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"),
            new DiscordWebhook("RemoveWarn", "", "", "Name: {name}, SteamID: {steamid}, Remover: {punisher}")
        ];
    }
}