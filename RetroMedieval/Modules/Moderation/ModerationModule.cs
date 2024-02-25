using RetroMedieval.Models.Moderation;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;

namespace RetroMedieval.Modules.Moderation;

[ModuleInformation("Moderation")]
[ModuleStorage<MySqlSaver<Warn>>("Warns")]
[ModuleStorage<MySqlSaver<Mute>>("Mutes")]
[ModuleStorage<MySqlSaver<Kick>>("Kicks")]
[ModuleStorage<MySqlSaver<Ban>>("Bans")]
internal class ModerationModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public void Ban(Ban ban, bool user_online = true)
    {
        if (!GetStorage<MySqlSaver<Ban>>(out var bans_storage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (!bans_storage.StartQuery().Insert(ban).ExecuteSql())
        {
            Logger.LogError("Could not enter ban into [BansStorage]");
            return;
        }

        if (user_online)
        {
            SDG.Unturned.Provider.kick(new CSteamID(ban.TargetID),
                $"[BAN] Reason: {ban.Reason} Time Left: {ban.TimeLeftString}");
        }


        if (ban.PunisherID == 0)
        {
            Logger.Log(
                $"Banned {UnturnedPlayer.FromCSteamID(new CSteamID(ban.TargetID)).DisplayName} for reason {ban.Reason} with time length: {ban.TimeLeftString}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(ban.PunisherID)),
                $"Banned {UnturnedPlayer.FromCSteamID(new CSteamID(ban.TargetID)).DisplayName} for reason {ban.Reason} with time length: {ban.TimeLeftString}");
        }
    }

    public void Kick(Kick kick, bool user_online = true)
    {
        if (!GetStorage<MySqlSaver<Kick>>(out var kicks_storage))
        {
            Logger.LogError("Could not gather storage [KicksStorage]");
            return;
        }

        if (!kicks_storage.StartQuery().Insert(kick).ExecuteSql())
        {
            Logger.LogError("Could not enter kick into [KicksStorage]");
            return;
        }

        if (user_online)
        {
            SDG.Unturned.Provider.kick(new CSteamID(kick.TargetID), $"[KICK] Reason: {kick.Reason}");
        }

        if (kick.PunisherID == 0)
        {
            Logger.Log(
                $"Kicked {UnturnedPlayer.FromCSteamID(new CSteamID(kick.TargetID)).DisplayName} for reason {kick.Reason}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(kick.PunisherID)),
                $"Kicked {UnturnedPlayer.FromCSteamID(new CSteamID(kick.TargetID)).DisplayName} for reason {kick.Reason}");
        }
    }

    public void Mute(Mute mute, bool user_online = true)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutes_storage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (!mutes_storage.StartQuery().Insert(mute).ExecuteSql())
        {
            Logger.LogError("Could not enter mute into [MutesStorage]");
            return;
        }

        if (user_online)
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(mute.TargetID)),
                $"[MUTE] Reason: {mute.Reason} Time Left: {mute.TimeLeftString}");
        }

        if (mute.PunisherID == 0)
        {
            Logger.Log(
                $"Muted {UnturnedPlayer.FromCSteamID(new CSteamID(mute.TargetID)).DisplayName} for reason {mute.Reason} with time length: {mute.TimeLeftString}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(mute.PunisherID)),
                $"Muted {UnturnedPlayer.FromCSteamID(new CSteamID(mute.TargetID)).DisplayName} for reason {mute.Reason} with time length: {mute.TimeLeftString}");
        }
    }

    public void Warn(Warn warn, bool user_online = true)
    {
        if (!GetStorage<MySqlSaver<Warn>>(out var warns_storage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (!warns_storage.StartQuery().Insert(warn).ExecuteSql())
        {
            Logger.LogError("Could not enter warn into [WarnsStorage]");
            return;
        }

        if (user_online)
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(warn.TargetID)), $"[WARN] Reason: {warn.Reason}");
        }

        if (warn.PunisherID == 0)
        {
            Logger.Log(
                $"Warned {UnturnedPlayer.FromCSteamID(new CSteamID(warn.TargetID)).DisplayName} for reason {warn.Reason}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(warn.PunisherID)),
                $"Warned {UnturnedPlayer.FromCSteamID(new CSteamID(warn.TargetID)).DisplayName} for reason {warn.Reason}");
        }
    }
}