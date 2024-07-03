using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation.Commands;

internal class BansCommand : IRocketCommand
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
        
        moderationModule.Bans(caller, targetsID);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "bans";
    public string Help => "Lists all of the bans for a user";
    public string Syntax => "bans <player name | player id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}