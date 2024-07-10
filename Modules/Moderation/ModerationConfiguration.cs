using Moderation.Models;
using RetroMedieval.Modules.Configuration;

namespace Moderation;

internal class ModerationConfiguration : IConfig
{
    public DiscordWebhook BanWebhook { get; set; }
    public DiscordWebhook UnbanWebhook { get; set; }
    public DiscordWebhook KickWebhook { get; set; }
    public DiscordWebhook WarnWebhook { get; set; }
    public DiscordWebhook RemoveWarnWebhook { get; set; }
    public DiscordWebhook MuteWebhook { get; set; }
    public DiscordWebhook UnmuteWebhook { get; set; }
    
    public void LoadDefaults()
    {
        BanWebhook = new DiscordWebhook("Ban", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}");
        UnbanWebhook = new DiscordWebhook("Unban", "", "",  "Name: {name}, SteamID: {steamid}, Unbanner: {punisher}" );
        MuteWebhook = new DiscordWebhook("Mute", "", "", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}");
        UnmuteWebhook = new DiscordWebhook("Unmute", "", "",  "Name: {name}, SteamID: {steamid}, Unmuter: {punisher}" );
        KickWebhook = new DiscordWebhook("Kick", "", "",  "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}" );
        WarnWebhook = new DiscordWebhook("Warn", "", "",  "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}" );
        RemoveWarnWebhook = new DiscordWebhook("RemoveWarn", "", "",  "Name: {name}, SteamID: {steamid}, Remover: {punisher}" );
    }
}