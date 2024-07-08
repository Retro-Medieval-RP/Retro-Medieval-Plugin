using System;
using JetBrains.Annotations;
using Moderation.Models;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Shared.Events;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation;

[ModuleInformation("Moderation")]
[ModuleStorage<MySqlSaver<Warn>>("Warns")]
[ModuleStorage<MySqlSaver<Mute>>("Mutes")]
[ModuleStorage<MySqlSaver<Kick>>("Kicks")]
[ModuleStorage<MySqlSaver<Ban>>("Bans")]
[ModuleStorage<MySqlSaver<ModerationPlayer>>("Players")]
internal class ModerationModule([NotNull] string directory) : Module(directory)
{
    public override void Load()
    {
        PlayerJoinEventEventPublisher.PlayerJoinEventEvent += OnPlayerJoined;
        PlayerVoiceEventEventPublisher.PlayerVoiceEventEvent += OnVoice;
        ChatEventEventPublisher.ChatEventEvent += OnUserMessage;
    }

    public override void Unload()
    {
        PlayerJoinEventEventPublisher.PlayerJoinEventEvent -= OnPlayerJoined;
        PlayerVoiceEventEventPublisher.PlayerVoiceEventEvent -= OnVoice;
        ChatEventEventPublisher.ChatEventEvent -= OnUserMessage;
    }

    protected override void OnTimerTick()
    {
        if (!GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        var bans = bansStorage
            .StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "BanLength", "BanOver")
            .Where(("BanOver", false))
            .Finalise()
            .Query<Ban>();

        var mutes = mutesStorage
            .StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "MuteLength", "MuteOver")
            .Where(("MuteOver", false))
            .Finalise()
            .Query<Mute>();

        foreach (var ban in bans)
        {
            if (ban.IsExpired)
            {
                bansStorage
                    .StartQuery()
                    .Update(("BanOver", true))
                    .Where(("PunishmentID", ban.PunishmentID))
                    .Finalise()
                    .ExecuteSql();
            }
        }

        foreach (var mute in mutes)
        {
            if (mute.IsExpired)
            {
                mutesStorage
                    .StartQuery()
                    .Update(("MuteOver", true))
                    .Where(("PunishmentID", mute.PunishmentID))
                    .Finalise()
                    .ExecuteSql();
            }
        }
    }

    private void OnUserMessage(ChatEventEventArgs e, ref bool allow)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (mutesStorage
                .StartQuery()
                .Count()
                .Where(("TargetID", e.Sender.m_SteamID), ("MuteOver", false))
                .Finalise()
                .QuerySingle<int>() > 0)
        {
            allow = false;
        }
    }

    private void OnVoice(PlayerVoiceEventEventArgs e, ref bool allow)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (mutesStorage.StartQuery().Count().Where(("TargetID", e.Sender.CSteamID.m_SteamID), ("MuteOver", false))
                .Finalise().QuerySingle<int>() > 0)
        {
            allow = false;
        }
    }

    private void OnPlayerJoined(PlayerJoinEventEventArgs e, ref bool allow)
    {
            
        if (!GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (bansStorage
                .StartQuery()
                .Count()
                .Where(("TargetID", e.Player.CSteamID.m_SteamID), ("BanOver", false))
                .Finalise()
                .QuerySingle<int>() <= 0)
        {
            if (!DoesPlayerAlreadyExist(e.Player.CSteamID.m_SteamID))
            {
                InsertPlayer(e.Player);
                return;
            }
                
            var storedUser = GetPlayer(e.Player.CSteamID.m_SteamID);
            if (storedUser.DisplayName != e.Player.DisplayName)
            {
                UpdateDisplayName(e.Player.CSteamID.m_SteamID, e.Player.DisplayName);
            }

            UpdateLastJoinDate(e.Player.CSteamID.m_SteamID);
            return;
        }

        var ban = bansStorage.StartQuery().Select("Reason", "BanLength", "PunishmentGiven")
            .Where(("TargetID", e.Player.CSteamID.m_SteamID)).Finalise().QuerySingle<Ban>();

        Provider.kick(e.Player.CSteamID,
            $"[BAN] Reason: {ban.Reason} Time Left: {ban.TimeLeftString}");
    }

    public void Ban(Ban ban, bool userOnline = true)
    {
        if (!GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (!bansStorage.StartQuery().Insert(ban).ExecuteSql())
        {
            Logger.LogError("Could not enter ban into [BansStorage]");
            return;
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

        if (userOnline)
        {
            Provider.kick(new CSteamID(ban.TargetID),
                $"[BAN] Reason: {ban.Reason} Time Left: {ban.TimeLeftString}");
        }
    }

    public void Kick(Kick kick, bool userOnline = true)
    {
        if (!GetStorage<MySqlSaver<Kick>>(out var kicksStorage))
        {
            Logger.LogError("Could not gather storage [KicksStorage]");
            return;
        }

        if (!kicksStorage.StartQuery().Insert(kick).ExecuteSql())
        {
            Logger.LogError("Could not enter kick into [KicksStorage]");
            return;
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

        if (userOnline)
        {
            Provider.kick(new CSteamID(kick.TargetID), $"[KICK] Reason: {kick.Reason}");
        }
    }

    public void Mute(Mute mute, bool userOnline = true)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (!mutesStorage.StartQuery().Insert(mute).ExecuteSql())
        {
            Logger.LogError("Could not enter mute into [MutesStorage]");
            return;
        }

        if (userOnline)
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

    public void Warn(Warn warn, bool userOnline = true)
    {
        if (!GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (!warnsStorage.StartQuery().Insert(warn).ExecuteSql())
        {
            Logger.LogError("Could not enter warn into [WarnsStorage]");
            return;
        }

        if (userOnline)
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

    public void RemoveWarn(IRocketPlayer caller, string warnID)
    {
        if (!GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (warnsStorage.StartQuery().Count().Where(("PunishmentID", warnID), ("WarnRemoved", false)).Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!warnsStorage.StartQuery().Update(("WarnRemoved", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update warn in [WarnsStorage] for warn id: {warnID}");
                return;
            }

            UnturnedChat.Say(caller, $"Warn ({warnID}) has been removed.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active warn or a warn with the id: {warnID}", Color.red);
        }
    }

    public void Unmute(IRocketPlayer caller, string muteID)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (mutesStorage.StartQuery().Count().Where(("PunishmentID", muteID), ("MuteOver", false)).Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!mutesStorage.StartQuery().Update(("MuteOver", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update mute in [MutesStorage] for mute id: {muteID}");
                return;
            }

            UnturnedChat.Say(caller, $"Mute ({muteID}) has been undone.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active mute or a mute with the id: {muteID}", Color.red);
        }
    }

    public void Unban(IRocketPlayer caller, string banID)
    {
        if (!GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (bansStorage.StartQuery().Count().Where(("PunishmentID", banID), ("BanOver", false)).Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!bansStorage.StartQuery().Update(("BanOver", true)).Finalise().ExecuteSql())
            {
                Logger.LogError($"Could not update ban in [BansStorage] for ban id: {banID}");
                return;
            }

            UnturnedChat.Say(caller, $"Ban ({banID}) has been removed.");
        }
        else
        {
            UnturnedChat.Say(caller, $"Could not find an active ban or a ban with the id: {banID}", Color.red);
        }
    }

    public void Bans(IRocketPlayer caller, ulong targetsID)
    {
        if (!GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        var bans = bansStorage
            .StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "BanLength", "BanOver")
            .Where(("BanOver", false), ("TargetID", targetsID))
            .Finalise()
            .Query<Ban>();
        
        UnturnedChat.Say(caller, "Bans:");
        foreach (var ban in bans)
        {
            UnturnedChat.Say(caller,
                $"BanID: {ban.PunishmentID} | Ban Reason: {ban.Reason} | Ban Expire Time: {ban.TimeLeftString} | Ban Given: {ban.PunishmentGiven}");
        }
    }

    public void Warns(IRocketPlayer caller, ulong targetsID)
    {
        if (!GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        var warns = warnsStorage
            .StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "WarnRemoved")
            .Where(("WarnRemoved", false), ("TargetID", targetsID))
            .Finalise()
            .Query<Warn>();

        UnturnedChat.Say(caller, "Warns:");
        foreach (var warn in warns)
        {
            UnturnedChat.Say(caller,
                $"WarnID: {warn.PunishmentID} | Warn Reason: {warn.Reason} | Warn Given: {warn.PunishmentGiven}");
        }
    }

    public void Mutes(IRocketPlayer caller, ulong targetsID)
    {
        if (!GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        var mutes = mutesStorage
            .StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "MuteLength", "MuteOver")
            .Where(("MuteOver", false), ("TargetID", targetsID))
            .Finalise()
            .Query<Mute>();

        UnturnedChat.Say(caller, "Mutes:");
        foreach (var mute in mutes)
        {
            UnturnedChat.Say(caller,
                $"MuteID: {mute.PunishmentID} | Mute Reason: {mute.Reason} | Mute Expire Time: {mute.TimeLeftString} | Mute Given: {mute.PunishmentGiven}");
        }
    }

    public ModerationPlayer GetPlayer(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            return playersStorage.StartQuery().Select("*").Where(("PlayerID", playerId)).Finalise()
                .QuerySingle<ModerationPlayer>();
        }
            
        Logger.LogError("Could not gather storage [PlayersStorage]");
        return null;
    }

    private bool DoesPlayerAlreadyExist(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            return playersStorage.StartQuery().Count().Where(("PlayerID", playerId)).Finalise().QuerySingle<int>() > 0;
        }
            
        Logger.LogError("Could not gather storage [PlayersStorage]");
        return false;
    }

    private void UpdateLastJoinDate(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage.StartQuery().Update(("LastJoinDate", DateTime.Now)).Where(("PlayerID", playerId)).Finalise().ExecuteSql();
            return;
        }
            
        Logger.LogError("Could not gather storage [PlayersStorage]");
    }

    private void UpdateDisplayName(ulong playerId, string displayName)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage.StartQuery().Update(("DisplayName", displayName)).Where(("PlayerID", playerId)).Finalise().ExecuteSql();
            return;
        }
            
        Logger.LogError("Could not gather storage [PlayersStorage]");
    }

    private void InsertPlayer(UnturnedPlayer player)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage.StartQuery().Insert(new ModerationPlayer
            {
                PlayerID = player.CSteamID.m_SteamID,
                DisplayName = player.DisplayName,
                FirstJoinDate = DateTime.Now,
                LastJoinDate = DateTime.Now
            });
            return;
        }
            
        Logger.LogError("Could not gather storage [PlayersStorage]");
    }
}