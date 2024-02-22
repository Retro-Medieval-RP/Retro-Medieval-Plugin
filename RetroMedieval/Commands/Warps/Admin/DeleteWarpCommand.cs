using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Warps;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Warps.Admin;

internal class DeleteWarpCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<WarpsModule>(out var warps_module))
        {
            Logger.LogError("Could not find module [WarpsModule]!");
            return;   
        }

        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        warps_module.RemoveWarp(command[0], caller is ConsolePlayer ? null : caller as UnturnedPlayer);
    }
  
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletewarp";
    public string Help => "Deletes a warp from the server.";
    public string Syntax => "deletewarp <warp name>";
    public List<string> Aliases => [ "dwarp" ];
    public List<string> Permissions => [];
}