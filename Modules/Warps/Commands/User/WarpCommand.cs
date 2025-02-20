using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Warps.Commands.User;

internal class WarpCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<WarpsModule>(out var warpsModule))
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
        
        warpsModule.WarpUser(caller as UnturnedPlayer, command[0]);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "warp";
    public string Help => "Teleports a user to a pre specified location";
    public string Syntax => "warp <warp name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}