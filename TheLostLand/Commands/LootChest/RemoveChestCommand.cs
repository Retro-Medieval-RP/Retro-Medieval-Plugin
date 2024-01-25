using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Modules;
using TheLostLand.Modules.LootChest;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.LootChest;

internal class RemoveChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!int.TryParse(command[1], out var id))
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, "ID could not be parsed into an int", Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var loot_chest))
        {
            Logger.LogError("Could not find module [LootChestModule]!");
            return;
        }

        if (loot_chest.RemoveChest(command[0], id))
        {
            UnturnedChat.Say(caller, $"Removed chest ({id}) form zone: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, $"Could not remove chest ({id}) form zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "removechest";
    public string Help => "Removes a chest at an ID from a Zone.";
    public string Syntax => "removechest <zone name> <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}