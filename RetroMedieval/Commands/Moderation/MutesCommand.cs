using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Moderation;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Moderation;

internal class MutesCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        ulong targetsID;
        var targetPlayer = UnturnedPlayer.FromName(command[0]);
        if (targetPlayer == null)
        {
            if (!ulong.TryParse(command[0], out targetsID))
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            targetsID = targetPlayer.CSteamID.m_SteamID;
        }
        
        moderationModule.Mutes(caller, targetsID);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "mutes";
    public string Help => "Lists all of the mutes for a user";
    public string Syntax => "mutes <player name | player id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}