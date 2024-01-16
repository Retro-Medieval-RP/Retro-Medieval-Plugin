using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Core.Modules;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands;

internal class DeleteZoneCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
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

        if (module.DeleteZone(command[0]))
        {
            UnturnedChat.Say(caller, "Deleted zone: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, "Could not delete zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletezone";
    public string Help => "Deletes a zone on the server.";
    public string Syntax => "deletezone <name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}