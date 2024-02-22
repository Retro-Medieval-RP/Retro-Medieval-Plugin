using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Warps;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Warps.Admin;

internal class CreateWarpCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<WarpsModule>(out var warps_module))
        {
            Logger.LogError("Could not find module [WarpsModule]!");
            return;   
        }
        
        if (caller is ConsolePlayer)
        {
            if (command.Length < 4)
            {
                UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
                UnturnedChat.Say(caller, ConsoleSyntax, Color.red);
                return;
            }

            if (!float.TryParse(command[1], out var x) || !float.TryParse(command[2], out var y) || !float.TryParse(command[3], out var z))
            {
                UnturnedChat.Say(caller, "Error: ", Color.red);
                UnturnedChat.Say(caller, "Could not parse either x, y, or z into a number.", Color.red);
                return;
            }

            var loc = new Vector3(x, y, z);
            
            warps_module.AddWarp(command[0], loc, 0);
            return;
        }

        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, UserSyntax, Color.red);
            return;
        }

        var player = caller as UnturnedPlayer;
        warps_module.AddWarp(command[0], player!.Position, 0, player);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "createwarp";
    public string Help => "Creates a warp location.";
    public string Syntax => "";
    public List<string> Aliases => [ "cwarp" ];
    public List<string> Permissions => [];

    private string ConsoleSyntax => "createwarp <warp name> <x> <y> <z>";
    private string UserSyntax => "createwarp <warp name>";
}