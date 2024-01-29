﻿using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Modules;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zones;

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
        
        if (zones_module.DeleteZone(command[0]))
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