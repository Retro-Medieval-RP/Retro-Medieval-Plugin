using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.LootChest;
using RetroMedieval.Modules.Zones;
using RetroMedieval.Utils;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.LootChest;

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

        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones_module))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;   
        }

        if (!zones_module.Exists(command[0]))
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
        
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var loot_chest))
        {
            Logger.LogError("Could not find module [LootChestModule]!");
            return;
        }

        if (loot_chest.AddChest(command[0], new Vector3(drop.model.position.x, ((UnturnedPlayer)caller).Position.y, drop.model.position.z), drop.model.rotation, out var node_id))
        {
            UnturnedChat.Say(caller, $"Added chest to zone {command[0]} with id: " + node_id);
            return;
        }

        UnturnedChat.Say(caller, "Could not add chest to zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addchest";
    public string Help => "Adds a chest at a location and at an angle.";
    public string Syntax => "addchest <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}