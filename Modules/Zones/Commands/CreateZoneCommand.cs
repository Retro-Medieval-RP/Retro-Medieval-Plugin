﻿using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Zones.Commands;

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
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zonesModule))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            return;
        }

        if (zonesModule.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, $"Zone {command[0]} does already exist!", Color.red);
            return;
        }
        
        if (zonesModule.CreateZone(command[0]))
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