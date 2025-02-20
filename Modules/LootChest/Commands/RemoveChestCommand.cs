﻿using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Zones;
using Logger = Rocket.Core.Logging.Logger;

namespace LootChest.Commands;

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
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zonesModule))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;   
        }
        
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var lootChest))
        {
            Logger.LogError("Could not find module [LootChestModule]!");
            return;
        }
        
        if (!zonesModule.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, $"Zone {command[0]} does not exist!", Color.red);
            return;
        }
        
        if (lootChest.RemoveChest(command[0], id))
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