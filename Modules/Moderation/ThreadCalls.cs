using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Moderation.Models;
using RetroMedieval.Savers.MySql;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation;

internal static class ThreadCalls
{
    internal static Thread BanCheck => new(BanCheckStart);
    internal static Thread MuteCheck => new(MuteCheckStart);
    internal static Thread PlayerJoin => new(PlayerJoinStart);
    internal static Thread PlayerBan => new(PlayerBanStart);
    internal static Thread PlayerKick => new(PlayerKickStart);
    internal static Thread PlayerMute => new(PlayerMuteStart);
    internal static Thread PlayerWarn => new(PlayerWarnStart);
    internal static Thread Warns => new(WarnsStart);
    internal static Thread Mutes => new(MutesStart);

    private static async void BanCheckStart(object module)
    {
        var moderationModule = module as ModerationModule;
        if (!moderationModule!.GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        var bans = await bansStorage.StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "BanLength", "BanOver")
            .Where(("BanOver", false))
            .Finalise()
            .Query<Ban>();

        foreach (var ban in bans)
        {
            if (ban.IsExpired)
            {
                bansStorage.StartQuery()
                    .Update(("BanOver", true))
                    .Where(("PunishmentID", ban.PunishmentID))
                    .Finalise()
                    .ExecuteSql();
            }
        }
    }
    
    private static async void MuteCheckStart(object module)
    {
        var moderationModule = module as ModerationModule;
        if (!moderationModule!.GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }


        var mutes = await mutesStorage.StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "MuteLength", "MuteOver")
            .Where(("MuteOver", false))
            .Finalise()
            .Query<Mute>();


        foreach (var mute in mutes)
        {
            if (mute.IsExpired)
            {
                mutesStorage.StartQuery()
                    .Update(("MuteOver", true))
                    .Where(("PunishmentID", mute.PunishmentID))
                    .Finalise()
                    .ExecuteSql();
            }
        }
    }
    
    private static async void PlayerJoinStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, UnturnedPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (await bansStorage.StartQuery()
                .Count()
                .Where(("TargetID", v.Item2.CSteamID.m_SteamID), ("BanOver", false))
                .Finalise()
                .QuerySingle<int>() <= 0)
        {
            if (v.Item1.GetConfiguration<NameBlackListConfiguration>(out var nameBlacklist))
            {
                const string englishCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234";
                const string specialCharacters = @"`¬-_=+[{]};:'@#~,<.>/?|\!£$%^&*()""";
                
                if (nameBlacklist.BlacklistFilter.Any(x => Regex.IsMatch(v.Item2.DisplayName, x)))
                {
                    Provider.kick(v.Item2.CSteamID, $"Name: {v.Item2.DisplayName} | Has been matched with name blacklist, please change your name.");
                    return;
                }
                
                if (nameBlacklist.BlacklistNonEnglishCharacters)
                {
                    if (v.Item2.DisplayName.Any(x => !englishCharacters.Contains(x) || !specialCharacters.Contains(x)))
                    {
                        Provider.kick(v.Item2.CSteamID, $"Name: {v.Item2.DisplayName} | Contains non english characters, please change your name.");
                        return;
                    }
                }

                if (nameBlacklist.BlacklistSpecialCharacters)
                {
                    if (v.Item2.DisplayName.Any(x => specialCharacters.Contains(x)))
                    {
                        Provider.kick(v.Item2.CSteamID, $"Name: {v.Item2.DisplayName} | Contains special characters, please change your name.");
                        return;
                    }
                }
            }
            
            if (!await v.Item1.DoesPlayerAlreadyExist(v.Item2.CSteamID.m_SteamID))
            {
                v.Item1.InsertPlayer(v.Item2);
                return;
            }

            var storedUser = await v.Item1.GetPlayer(v.Item2.CSteamID.m_SteamID);
            if (storedUser.DisplayName != v.Item2.DisplayName)
            {
                v.Item1.UpdateDisplayName(v.Item2.CSteamID.m_SteamID, v.Item2.DisplayName);
            }

            v.Item1.UpdateLastJoinDate(v.Item2.CSteamID.m_SteamID);
            return;
        }

        var ban = await bansStorage.StartQuery()
            .Select("Reason", "BanLength", "PunishmentGiven")
            .Where(("TargetID", v.Item2.CSteamID.m_SteamID))
            .Finalise()
            .QuerySingle<Ban>();

        Provider.kick(v.Item2.CSteamID, $"[BAN] Reason: {ban.Reason} Time Left: {ban.TimeLeftString}");
    }
    
    private static async void PlayerBanStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, Ban, bool>;
        if (!v!.Item1.GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (!bansStorage.StartQuery().Insert(v.Item2).ExecuteSql())
        {
            Logger.LogError("Could not enter ban into [BansStorage]");
            return;
        }

        if (v.Item2.PunisherID == 0)
        {
            Logger.Log($"Banned {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason} with time length: {v.Item2.TimeLeftString}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.PunisherID)), $"Banned {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason} with time length: {v.Item2.TimeLeftString}");
        }

        if (v.Item3)
        {
            Provider.kick(new CSteamID(v.Item2.TargetID), $"[BAN] Reason: {v.Item2.Reason} Time Left: {v.Item2.TimeLeftString}");
        }

        var target = await v.Item1.GetPlayer(v.Item2.TargetID);
        var punisher = v.Item2.PunisherID == 0
            ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
            : await v.Item1.GetPlayer(v.Item2.PunisherID);

        v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), v.Item2.Reason, v.Item2.DurationString], ModerationActionType.Ban);
    }

    private static async void PlayerKickStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, Kick, bool>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kick>>(out var kicksStorage))
        {
            Logger.LogError("Could not gather storage [KicksStorage]");
            return;
        }

        if (!kicksStorage.StartQuery()
                .Insert(v.Item2)
                .ExecuteSql())
        {
            Logger.LogError("Could not enter kick into [KicksStorage]");
            return;
        }

        if (v.Item2.PunisherID == 0)
        {
            Logger.Log($"Kicked {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.PunisherID)), $"Kicked {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason}");
        }

        if (v.Item3)
        {
            Provider.kick(new CSteamID(v.Item2.TargetID), $"[KICK] Reason: {v.Item2.Reason}");
        }

        var target = await v.Item1.GetPlayer(v.Item2.TargetID);
        var punisher = v.Item2.PunisherID == 0
            ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
            : await v.Item1.GetPlayer(v.Item2.PunisherID);

        v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), v.Item2.Reason], ModerationActionType.Kick);
    }

    private static async void PlayerMuteStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, Mute, bool>;
        if (!v!.Item1.GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (!mutesStorage.StartQuery()
                .Insert(v.Item2)
                .ExecuteSql())
        {
            Logger.LogError("Could not enter mute into [MutesStorage]");
            return;
        }

        if (v.Item3)
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)), $"[MUTE] Reason: {v.Item2.Reason} Time Left: {v.Item2.TimeLeftString}");
        }

        if (v.Item2.PunisherID == 0)
        {
            Logger.Log($"Muted {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason} with time length: {v.Item2.TimeLeftString}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.PunisherID)), $"Muted {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason} with time length: {v.Item2.TimeLeftString}");
        }

        var target = await v.Item1.GetPlayer(v.Item2.TargetID);
        var punisher = v.Item2.PunisherID == 0
            ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
            : await v.Item1.GetPlayer(v.Item2.PunisherID);

        v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), v.Item2.Reason, v.Item2.DurationString], ModerationActionType.Mute);
    }

    private static async void PlayerWarnStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, Warn, bool>;
        if (!v!.Item1.GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (!warnsStorage.StartQuery()
                .Insert(v.Item2)
                .ExecuteSql())
        {
            Logger.LogError("Could not enter warn into [WarnsStorage]");
            return;
        }

        if (v.Item3)
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)), $"[WARN] Reason: {v.Item2.Reason}");
        }

        if (v.Item2.PunisherID == 0)
        {
            Logger.Log($"Warned {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason}");
        }
        else
        {
            UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.PunisherID)), $"Warned {UnturnedPlayer.FromCSteamID(new CSteamID(v.Item2.TargetID)).DisplayName} for reason {v.Item2.Reason}");
        }

        var target = await v.Item1.GetPlayer(v.Item2.TargetID);
        var punisher = v.Item2.PunisherID == 0
            ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
            : await v.Item1.GetPlayer(v.Item2.PunisherID);

        v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), v.Item2.Reason], ModerationActionType.Warn);
    }

    internal static readonly Thread RemoveWarn = new(RemoveWarnStart);

    private static async void RemoveWarnStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, string, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        if (await warnsStorage.StartQuery()
                .Count()
                .Where(("PunishmentID", v.Item2), ("WarnRemoved", false))
                .Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!warnsStorage.StartQuery()
                    .Update(("WarnRemoved", true))
                    .Finalise()
                    .ExecuteSql())
            {
                Logger.LogError($"Could not update warn in [WarnsStorage] for warn id: {v.Item2}");
                return;
            }

            UnturnedChat.Say(v.Item3, $"Warn ({v.Item2}) has been removed.");

            var warn = await warnsStorage.StartQuery()
                .Select("*")
                .Where(("PunishmentID", v.Item2))
                .Finalise()
                .QuerySingle<Warn>();
            var target = await v.Item1.GetPlayer(warn.TargetID);
            var punisher = v.Item3 is ConsolePlayer
                ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
                : await v.Item1.GetPlayer(((UnturnedPlayer)v.Item3).CSteamID.m_SteamID);

            v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), warn.Reason], ModerationActionType.RemoveWarn);
        }
        else
        {
            UnturnedChat.Say(v.Item3, $"Could not find an active warn or a warn with the id: {v.Item2}", Color.red);
        }
    }

    internal static readonly Thread UnmutePlayer = new(UnmutePlayerStart);

    private static async void UnmutePlayerStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, string, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        if (await mutesStorage.StartQuery()
                .Count()
                .Where(("PunishmentID", v.Item2), ("MuteOver", false))
                .Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!mutesStorage.StartQuery()
                    .Update(("MuteOver", true))
                    .Finalise()
                    .ExecuteSql())
            {
                Logger.LogError($"Could not update mute in [MutesStorage] for mute id: {v.Item2}");
                return;
            }

            var mute = await mutesStorage.StartQuery()
                .Select("*")
                .Where(("PunishmentID", v.Item2))
                .Finalise()
                .QuerySingle<Mute>();

            var target = await v.Item1.GetPlayer(mute.TargetID);

            var punisher = v.Item3 is ConsolePlayer
                ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
                : await v.Item1.GetPlayer(((UnturnedPlayer)v.Item3).CSteamID.m_SteamID);

            v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), mute.Reason], ModerationActionType.Unmute);
            UnturnedChat.Say(v.Item3, $"Mute ({v.Item2}) has been undone.");
        }
        else
        {
            UnturnedChat.Say(v.Item3, $"Could not find an active mute or a mute with the id: {v.Item2}", Color.red);
        }
    }

    internal static readonly Thread UnbanPlayer = new(UnbanPlayerStart);

    private static async void UnbanPlayerStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, string, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        if (await bansStorage.StartQuery()
                .Count()
                .Where(("PunishmentID", v.Item2), ("BanOver", false))
                .Finalise()
                .QuerySingle<int>() > 0)
        {
            if (!bansStorage.StartQuery()
                    .Update(("BanOver", true))
                    .Finalise()
                    .ExecuteSql())
            {
                Logger.LogError($"Could not update ban in [BansStorage] for ban id: {v.Item2}");
                return;
            }

            var ban = await bansStorage.StartQuery()
                .Select("*")
                .Where(("PunishmentID", v.Item2))
                .Finalise()
                .QuerySingle<Ban>();
            var target = await v.Item1.GetPlayer(ban.TargetID);
            var punisher = v.Item3 is ConsolePlayer
                ? new ModerationPlayer { DisplayName = "Console", FirstJoinDate = DateTime.Now, LastJoinDate = DateTime.Now, PlayerID = 0 }
                : await v.Item1.GetPlayer(((UnturnedPlayer)v.Item3).CSteamID.m_SteamID);

            v.Item1.DiscordService.SendMessage([target.DisplayName, target.PlayerID.ToString(), punisher.DisplayName, punisher.PlayerID.ToString(), ban.Reason], ModerationActionType.Unban);

            UnturnedChat.Say(v.Item3, $"Ban ({v.Item2}) has been removed.");
        }
        else
        {
            UnturnedChat.Say(v.Item3, $"Could not find an active ban or a ban with the id: {v.Item2}", Color.red);
        }
    }

    internal static readonly Thread Bans = new(BansStart);

    private static async void BansStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, ulong, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Ban>>(out var bansStorage))
        {
            Logger.LogError("Could not gather storage [BansStorage]");
            return;
        }

        var bans = await bansStorage.StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "BanLength", "BanOver")
            .Where(("BanOver", false), ("TargetID", v.Item2))
            .Finalise()
            .Query<Ban>();

        UnturnedChat.Say(v.Item3, "Bans:");
        foreach (var ban in bans)
        {
            UnturnedChat.Say(v.Item3, $"BanID: {ban.PunishmentID} | Ban Reason: {ban.Reason} | Ban Expire Time: {ban.TimeLeftString} | Ban Given: {ban.PunishmentGiven}");
        }
    }
    
    private static async void WarnsStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, ulong, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Warn>>(out var warnsStorage))
        {
            Logger.LogError("Could not gather storage [WarnsStorage]");
            return;
        }

        var warns = await warnsStorage.StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "WarnRemoved")
            .Where(("WarnRemoved", false), ("TargetID", v.Item2))
            .Finalise()
            .Query<Warn>();

        UnturnedChat.Say(v.Item3, "Warns:");
        foreach (var warn in warns)
        {
            UnturnedChat.Say(v.Item3, $"WarnID: {warn.PunishmentID} | Warn Reason: {warn.Reason} | Warn Given: {warn.PunishmentGiven}");
        }
    }

    private static async void MutesStart(object vals)
    {
        var v = vals as Tuple<ModerationModule, ulong, IRocketPlayer>;
        if (!v!.Item1.GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            Logger.LogError("Could not gather storage [MutesStorage]");
            return;
        }

        var mutes = await mutesStorage.StartQuery()
            .Select("PunishmentID", "PunisherID", "TargetID", "PunishmentGiven", "Reason", "MuteLength", "MuteOver")
            .Where(("MuteOver", false), ("TargetID", v.Item2))
            .Finalise()
            .Query<Mute>();

        UnturnedChat.Say(v.Item3, "Mutes:");
        foreach (var mute in mutes)
        {
            UnturnedChat.Say(v.Item3, $"MuteID: {mute.PunishmentID} | Mute Reason: {mute.Reason} | Mute Expire Time: {mute.TimeLeftString} | Mute Given: {mute.PunishmentGiven}");
        }
    }

    internal static readonly Thread InsertUser = new(vals =>
    {
        var v = vals as Tuple<ModerationModule, UnturnedPlayer>;
        if (v!.Item1.GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage
                .StartQuery()
                .Insert(new ModerationPlayer
                {
                    PlayerID = v.Item2.CSteamID.m_SteamID,
                    DisplayName = v.Item2.DisplayName,
                    FirstJoinDate = DateTime.Now,
                    LastJoinDate = DateTime.Now
                })
                .ExecuteSql();
            return;
        }

        Logger.LogError("Could not gather storage [PlayersStorage]");
    });
}