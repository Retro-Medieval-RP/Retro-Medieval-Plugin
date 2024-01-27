using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Modules;
using TheLostLand.Modules.LootChest;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.LootChest;

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
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, $"Zone {command[0]} does not exist!", Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var loot_chest))
        {
            Logger.LogError("Could not find module [LootChestModule]!");
            return;
        }

        if (loot_chest.AddChest(command[0], ((UnturnedPlayer)caller).Position, ((UnturnedPlayer)caller).Player.look.transform.rotation, out var node_id))
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