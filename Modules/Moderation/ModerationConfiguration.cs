using System.Collections.Generic;
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
            new DiscordWebhook("Ban", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"){ Title = "Ban Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("Unban", "", "d76d55", "Name: {name}, SteamID: {steamid}, Unbanner: {punisher}"){ Title = "Unban Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("Mute", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"){ Title = "Mute Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("Unmute", "", "d76d55", "Name: {name}, SteamID: {steamid}, Unmuter: {punisher}"){ Title = "Unmute Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("Kick", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"){ Title = "Kick Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("Warn", "", "d76d55", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}"){ Title = "Warn Log", Url = "https://imgur.com/pXuEbxf" },
            new DiscordWebhook("RemoveWarn", "", "d76d55", "Name: {name}, SteamID: {steamid}, Remover: {punisher}"){ Title = "Remove Warn Log", Url = "https://imgur.com/pXuEbxf" }
        ];
    }
}