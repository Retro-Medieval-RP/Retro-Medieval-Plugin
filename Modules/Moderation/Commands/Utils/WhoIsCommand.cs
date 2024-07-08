using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Utils;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation.Commands.Utils;

internal class WhoIsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        var player = caller as UnturnedPlayer;
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE | RayMasks.STRUCTURE);
        if (!result.RaycastHit)
        {
            return;
        }

        ulong ownerId = 0;
        switch (result.Barricade)
        {
            case null when result.Structure == null:
                UnturnedChat.Say(caller, "Could not find a barricade or structure from raycast.", Color.red);
                return;
            case not null:
            {
                ownerId = result.Barricade.owner;
                break;
            }
            case null:
            {
                ownerId = result.Structure.owner;
                break;
            }
        }
        
        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        if (ownerId == 0)
        {
            UnturnedChat.Say(caller, "Owner is: Console (0)");
            return;
        }
        
        var ownerPlayer = moderationModule.GetPlayer(ownerId);
        UnturnedChat.Say(caller, $"Owner is: {ownerPlayer.DisplayName} ({ownerPlayer.PlayerID})");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "whois";
    public string Help => "This will tell the user who the owner of the barricade or structure is that they are looking at.";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}