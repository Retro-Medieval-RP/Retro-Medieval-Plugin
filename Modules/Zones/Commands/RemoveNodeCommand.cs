﻿using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Zones.Commands;

internal class RemoveNodeCommand : IRocketCommand
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
        
        if (!zonesModule.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, $"Zone {command[0]} does already exist!", Color.red);
            return;
        }
        
        if (zonesModule.RemoveNode(command[0], id))
        {
            UnturnedChat.Say(caller, $"Removed node ({id}) for zone: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, $"Could not remove node ({id}) for zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "removenode";
    public string Help => "Removes a node from a zone.";
    public string Syntax => "removenode <zone name> <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}