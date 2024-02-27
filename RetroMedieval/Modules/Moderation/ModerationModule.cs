using RetroMedieval.Models.Moderation;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

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

    public void RemoveWarn(IRocketPlayer caller, string warn_id)
    {
        if (!GetStorage<MySqlSaver<Warn>>(out var warns_storage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (warns_storage.StartQuery().Count().Where(("PunishmentID", warn_id), ("WarnRemoved", false)).Finalise().QuerySql<int>() > 0)
        {
            if (!warns_storage.StartQuery().Update(("WarnRemoved", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update warn in [WarnsStorage] for warn id: {warn_id}");
                return;
            }
            
            UnturnedChat.Say(caller, $"Warn ({warn_id}) has been removed.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active warn or a warn with the id: {warn_id}", Color.red);
        }
    }

    public void Unmute(IRocketPlayer caller, string mute_id)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutes_storage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (mutes_storage.StartQuery().Count().Where(("PunishmentID", mute_id), ("MuteOver", false)).Finalise().QuerySql<int>() > 0)
        {
            if (!mutes_storage.StartQuery().Update(("MuteOver", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update mute in [MutesStorage] for mute id: {mute_id}");
                return;
            }
            
            UnturnedChat.Say(caller, $"Mute ({mute_id}) has been undone.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active mute or a mute with the id: {mute_id}", Color.red);
        }
    }

    public void Unban(IRocketPlayer caller, string ban_id)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var bans_storage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (bans_storage.StartQuery().Count().Where(("PunishmentID", ban_id), ("BanOver", false)).Finalise().QuerySql<int>() > 0)
        {
            if (!bans_storage.StartQuery().Update(("BanOver", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update ban in [BansStorage] for ban id: {ban_id}");
                return;
            }
            
            UnturnedChat.Say(caller, $"Ban ({ban_id}) has been removed.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active ban or a ban with the id: {ban_id}", Color.red);
        }
    }
}