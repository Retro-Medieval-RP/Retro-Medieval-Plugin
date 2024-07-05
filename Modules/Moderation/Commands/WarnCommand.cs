using System;
using System.Collections.Generic;
using System.Linq;
using Moderation.Models;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation.Commands;

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

        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        var warn = new Warn
        {
            PunishmentID = Guid.NewGuid()
        };
        
        var targetPlayer = UnturnedPlayer.FromName(command[0]);
        if (targetPlayer == null)
        {
            if (ulong.TryParse(command[0], out var warnTargetID))
            {
                warn.TargetID = warnTargetID;
            }
            else
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            warn.TargetID = targetPlayer.CSteamID.m_SteamID;
        }
        
        warn.PunisherID = caller is ConsolePlayer ? 0 : ulong.Parse(caller.Id);
        warn.Reason = command.ElementAtOrDefault(1);
        warn.PunishmentGiven = DateTime.Now;
        
        moderationModule.Warn(warn, Provider.clients.Any(x => x.playerID.steamID.m_SteamID == warn.TargetID));
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "warn";
    public string Help => "Warns a user on the server.";
    public string Syntax => "warn <player name | player id> <reason>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}