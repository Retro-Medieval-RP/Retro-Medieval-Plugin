using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Modules;
using RetroMedieval.Utils;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Zones;
using Logger = Rocket.Core.Logging.Logger;

namespace LootChest.Commands;

internal class AddChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zonesModule))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;   
        }

        if (!zonesModule.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, $"Zone {command[0]} does not exist!", Color.red);
            return;
        }

        var result = Raycaster.RayCastPlayer((UnturnedPlayer)caller, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, "Please look at a barricade!", Color.red);
            return;
        }

        var drop = BarricadeManager.FindBarricadeByRootTransform(result.BarricadeRootTransform);
        
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var lootChest))
        {
            Logger.LogError("Could not find module [LootChestModule]!");
            return;
        }

        if (lootChest.AddChest(command[0], new Vector3(drop.model.position.x, ((UnturnedPlayer)caller).Position.y, drop.model.position.z), drop.model.rotation, string.Join("¬", command.Skip(1)), out var nodeID))
        {
            UnturnedChat.Say(caller, $"Added chest to zone {command[0]} with id: " + nodeID);
            return;
        }

        UnturnedChat.Say(caller, "Could not add chest to zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addchest";
    public string Help => "Adds a chest at a location and at an angle.";
    public string Syntax => "addchest <zone name> <flags>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}