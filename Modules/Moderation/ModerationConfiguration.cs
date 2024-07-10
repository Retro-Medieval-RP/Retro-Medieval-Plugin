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
            new DiscordWebhook("Ban", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"){ Title = "Ban Log" },
            new DiscordWebhook("Unban", "", "d76d55", "Name: {name}, SteamID: {steamid}, Unbanner: {punisher}"){ Title = "Unban Log" },
            new DiscordWebhook("Mute", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"){ Title = "Mute Log" },
            new DiscordWebhook("Unmute", "", "d76d55", "Name: {name}, SteamID: {steamid}, Unmuter: {punisher}"){ Title = "Unmute Log" },
            new DiscordWebhook("Kick", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"){ Title = "Kick Log" },
            new DiscordWebhook("Warn", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"){ Title = "Warn Log" },
            new DiscordWebhook("RemoveWarn", "", "d76d55", "Name: {name}, SteamID: {steamid}, Remover: {punisher}"){ Title = "Remove Warn Log" }
        ];
    }
}