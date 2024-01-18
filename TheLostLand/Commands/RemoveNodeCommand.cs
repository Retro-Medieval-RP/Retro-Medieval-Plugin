using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Core.Modules;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands;

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
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var module))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;
        }

        if (module.RemoveNode(command[0], id))
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