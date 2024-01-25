﻿using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Modules;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zones;

internal class CreateZoneCommand : IRocketCommand
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

        if (module.CreateZone(command[0]))
        {
            UnturnedChat.Say(caller, "Created zone: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, "Could not create zone: " + command[0], Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "createzone";
    public string Help => "Creates a zone with a specified name";
    public string Syntax => "createzone <name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}