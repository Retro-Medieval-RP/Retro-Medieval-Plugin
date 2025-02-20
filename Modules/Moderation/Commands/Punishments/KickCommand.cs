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

internal class KickCommand : IRocketCommand
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

        var kick = new Kick
        {
            PunishmentID = Guid.NewGuid()
        };
        
        var targetPlayer = UnturnedPlayer.FromName(command[0]);
        if (targetPlayer == null)
        {
            if (ulong.TryParse(command[0], out var kickTargetID))
            {
                if (UnturnedPlayer.FromCSteamID(new CSteamID(kickTargetID)) == null)
                {
                    UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                    return;
                }
                
                kick.TargetID = kickTargetID;
            }
            else
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            kick.TargetID = targetPlayer.CSteamID.m_SteamID;
        }
        
        kick.PunisherID = caller is ConsolePlayer ? 0 : ulong.Parse(caller.Id);
        kick.Reason = command.ElementAtOrDefault(1);
        kick.PunishmentGiven = DateTime.Now;
        
        moderationModule.Kick(kick);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kick";
    public string Help => "Kicks a user from the server.";
    public string Syntax => "kick <player name | player id> <reason>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}