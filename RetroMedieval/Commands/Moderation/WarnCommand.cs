using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Moderation;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Moderation;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Moderation;

internal class WarnCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderation_module))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        var warn = new Warn
        {
            PunishmentID = Guid.NewGuid()
        };;
        
        var target_player = UnturnedPlayer.FromName(command[0]);
        if (target_player == null)
        {
            if (ulong.TryParse(command[0], out var warn_target_id))
            {
                if (UnturnedPlayer.FromCSteamID(new CSteamID(warn_target_id)) == null)
                {
                    UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                    return;
                }
                
                warn.TargetID = warn_target_id;
            }
            else
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            warn.TargetID = target_player.CSteamID.m_SteamID;
        }
        
        warn.PunisherID = caller is ConsolePlayer ? 0 : ulong.Parse(caller.Id);
        warn.Reason = command.ElementAtOrDefault(1);
        warn.PunishmentGiven = DateTime.Now;
        
        moderation_module.Warn(warn);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "warn";
    public string Help => "Warns a user on the server.";
    public string Syntax => "warn <player name | player id> <reason>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}