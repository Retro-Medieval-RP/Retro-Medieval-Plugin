using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Core.Modules;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands;

internal class AddNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var module))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;
        }

        if (module.AddNode(command[0], ((UnturnedPlayer)caller).Position, out var node_id))
        {
            UnturnedChat.Say(caller, $"Added node to {command[0]} with id: " + node_id);
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