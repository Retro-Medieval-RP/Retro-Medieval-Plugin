using System.Collections.Generic;
using RetroMedieval.Utils;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Moderation.Commands.Utils;

internal class WreckBarricadeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        var player = caller as UnturnedPlayer;
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE | RayMasks.STRUCTURE);
        if (!result.RaycastHit)
        {
            return;
        }

        switch (result.Barricade)
        {
            case null when result.Structure == null:
                UnturnedChat.Say(caller, "Could not find a barricade or structure from raycast.", Color.red);
                return;
            case not null:
            {
                var drop = BarricadeManager.FindBarricadeByRootTransform(result.BarricadeRootTransform);
                BarricadeManager.tryGetRegion(result.BarricadeRootTransform, out var x, out var y, out var plant,
                    out _);
                BarricadeManager.destroyBarricade(drop, x, y, plant);
                UnturnedChat.Say(caller,
                    $"Successfully destroyed barricade: {result.Barricade.barricade.asset.FriendlyName}");
                return;
            }
            case null:
            {
                var drop = StructureManager.FindStructureByRootTransform(result.StructureRootTransform);
                StructureManager.tryGetRegion(result.StructureRootTransform, out var x, out var y, out _);
                StructureManager.destroyStructure(drop, x, y, Vector3.zero, false);
                UnturnedChat.Say(caller, $"Successfully destroyed structure: {result.Structure.structure.asset.FriendlyName}");
                return;
            }
        }
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "wb";
    public string Help => "Removes the buildable that the user is looking at";
    public string Syntax => "wb";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}