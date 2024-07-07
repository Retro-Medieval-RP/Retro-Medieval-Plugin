using System;
using System.Collections.Generic;
using System.Linq;
using Moderation.Models;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation.Commands.Punishments;

internal class MuteCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        var mute = new Mute
        {
            PunishmentID = Guid.NewGuid()
        };

        var targetPlayer = UnturnedPlayer.FromName(command[0]);
        if (targetPlayer == null)
        {
            if (ulong.TryParse(command[0], out var muteTargetID))
            {
                if (UnturnedPlayer.FromCSteamID(new CSteamID(muteTargetID)) == null)
                {
                    UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                    return;
                }
                
                mute.TargetID = muteTargetID;
            }
            else
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            mute.TargetID = targetPlayer.CSteamID.m_SteamID;
        }

        mute.PunisherID = caller is ConsolePlayer ? 0 : ulong.Parse(caller.Id);
        mute.Reason = command.ElementAtOrDefault(1);
        var length = ModerationAction.ConvertToBanDuration(command.Skip(2));
        mute.MuteLength = length ?? -1;
        mute.PunishmentGiven = DateTime.Now;
        
        moderationModule.Mute(mute);

    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "mute";
    public string Help => "Mutes a user on a server.";
    public string Syntax => "mute <player name | player id> <reason> <time span>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}