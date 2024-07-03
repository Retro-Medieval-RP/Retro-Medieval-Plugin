using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Zones;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Zones;

internal class AddNodeCommand : IRocketCommand
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
        
        if (zonesModule.AddNode(command[0], ((UnturnedPlayer)caller).Position, out var nodeID))
        {
            UnturnedChat.Say(caller, $"Added node to {command[0]} with id: " + nodeID);
            return;
        }
        
        UnturnedChat.Say(caller, "Could not add node to zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addnode";
    public string Help => "Adds a node to a zone.";
    public string Syntax => "addnode <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}