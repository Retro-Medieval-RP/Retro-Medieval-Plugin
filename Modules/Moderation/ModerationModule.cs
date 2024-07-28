using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Moderation.Models;
using Moderation.Services;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Shared.Events.Unturned;
using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation;

[ModuleInformation("Moderation")]
[ModuleConfiguration<ModerationConfiguration>("ModerationConfiguration")]
[ModuleStorage<MySqlSaver<Warn>>("Warns")]
[ModuleStorage<MySqlSaver<Mute>>("Mutes")]
[ModuleStorage<MySqlSaver<Kick>>("Kicks")]
[ModuleStorage<MySqlSaver<Ban>>("Bans")]
[ModuleStorage<MySqlSaver<ModerationPlayer>>("Players")]
internal class ModerationModule([NotNull] string directory) : Module(directory)
{
    internal DiscordService DiscordService { get; private set; }

    public override void Load()
    {
        DiscordService = new DiscordService();

        U.Events.OnPlayerConnected += OnPlayerConnected;

        PlayerVoiceEventPublisher.PlayerVoiceEventEvent += OnVoice;
        ChatEventPublisher.ChatEventEvent += OnUserMessage;
    }

    private void OnUserMessage(ChatEventArgs e, ref bool allow) =>
        allow = OnMessageAsync(e).Result;

    private void OnVoice(PlayerVoiceEventArgs e, ref bool allow) =>
        allow = OnVoiceAsync(e).Result;

    private void OnPlayerConnected(UnturnedPlayer player) => 
        ThreadCalls.PlayerJoin.Start(new Tuple<ModerationModule, UnturnedPlayer>(this, player));

    public override void Unload()
    {
        U.Events.OnPlayerConnected -= OnPlayerConnected;

        PlayerVoiceEventPublisher.PlayerVoiceEventEvent -= OnVoice;
        ChatEventPublisher.ChatEventEvent -= OnUserMessage;
    }

    protected override void OnTimerTick()
    {
        ThreadCalls.BanCheck.Start(this);
        ThreadCalls.MuteCheck.Start(this);
    }

    private async Task<bool> OnMessageAsync(ChatEventArgs e)
    {
        if (GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            return await mutesStorage
                .StartQuery()
                .Count()
                .Where(("TargetID", e.Sender.m_SteamID), ("MuteOver", false))
                .Finalise()
                .QuerySingle<int>() <= 0;
        }

        Logger.LogError("Could not gather storage [MutesStorage]");
        return true;
    }

    private async Task<bool> OnVoiceAsync(PlayerVoiceEventArgs e)
    {
        if (GetStorage<MySqlSaver<Mute>>(out var mutesStorage))
        {
            return await mutesStorage
                .StartQuery()
                .Count()
                .Where(("TargetID", e.Sender.CSteamID.m_SteamID), ("MuteOver", false))
                .Finalise()
                .QuerySingle<int>() <= 0;
        }

        Logger.LogError("Could not gather storage [MutesStorage]");
        return true;
    }

    public void Ban(Ban ban, bool userOnline = true) => 
        ThreadCalls.PlayerBan.Start(new Tuple<ModerationModule, Ban, bool>(this, ban, userOnline));
    
    public void Kick(Kick kick, bool userOnline = true) => 
        ThreadCalls.PlayerKick.Start(new Tuple<ModerationModule, Kick, bool>(this, kick, userOnline));

    public void Mute(Mute mute, bool userOnline = true) => 
        ThreadCalls.PlayerMute.Start(new Tuple<ModerationModule, Mute, bool>(this, mute, userOnline));

    public void Warn(Warn warn, bool userOnline = true) => 
        ThreadCalls.PlayerWarn.Start(new Tuple<ModerationModule, Warn, bool>(this, warn, userOnline));

    public void RemoveWarn(IRocketPlayer caller, string warnID) => 
        ThreadCalls.RemoveWarn.Start(new Tuple<ModerationModule, string, IRocketPlayer>(this, warnID, caller));

    public void Unmute(IRocketPlayer caller, string muteID) => 
        ThreadCalls.UnmutePlayer.Start(new Tuple<ModerationModule, string, IRocketPlayer>(this, muteID, caller));

    public void Unban(IRocketPlayer caller, string banID) => 
        ThreadCalls.UnbanPlayer.Start(new Tuple<ModerationModule, string, IRocketPlayer>(this, banID, caller));

    public void Bans(IRocketPlayer caller, ulong targetsID) => 
        ThreadCalls.Bans.Start(new Tuple<ModerationModule, ulong, IRocketPlayer>(this, targetsID, caller));

    public void Warns(IRocketPlayer caller, ulong targetsID) => 
        ThreadCalls.Warns.Start(new Tuple<ModerationModule, ulong, IRocketPlayer>(this, targetsID, caller));

    public void Mutes(IRocketPlayer caller, ulong targetsID) => 
        ThreadCalls.Mutes.Start(new Tuple<ModerationModule, ulong, IRocketPlayer>(this, targetsID, caller));

    internal async Task<ModerationPlayer> GetPlayer(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            return await playersStorage.StartQuery().Select("*").Where(("PlayerID", playerId)).Finalise()
                .QuerySingle<ModerationPlayer>();
        }

        Logger.LogError("Could not gather storage [PlayersStorage]");
        return null;
    }

    internal async Task<bool> DoesPlayerAlreadyExist(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            return await playersStorage
                .StartQuery()
                .Count()
                .Where(("PlayerID", playerId))
                .Finalise()
                .QuerySingle<int>() > 0;
        }

        Logger.LogError("Could not gather storage [PlayersStorage]");
        return false;
    }

    internal void UpdateLastJoinDate(ulong playerId)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage
                .StartQuery()
                .Update(("LastJoinDate", DateTime.Now))
                .Where(("PlayerID", playerId))
                .Finalise()
                .ExecuteSql();
            return;
        }

        Logger.LogError("Could not gather storage [PlayersStorage]");
    }

    internal void UpdateDisplayName(ulong playerId, string displayName)
    {
        if (GetStorage<MySqlSaver<ModerationPlayer>>(out var playersStorage))
        {
            playersStorage
                .StartQuery()
                .Update(("DisplayName", displayName))
                .Where(("PlayerID", playerId))
                .Finalise()
                .ExecuteSql();
            return;
        }

        Logger.LogError("Could not gather storage [PlayersStorage]");
    }

    internal void InsertPlayer(UnturnedPlayer player) => 
        ThreadCalls.InsertUser.Start(new Tuple<ModerationModule, UnturnedPlayer>(this, player));
}