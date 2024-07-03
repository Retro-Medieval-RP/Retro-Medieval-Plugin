using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Warps;
using Rocket.API;
using Rocket.Core.Logging;

namespace RetroMedieval.Commands.Warps.User;

internal class WarpsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<WarpsModule>(out var warpsModule))
        {
            Logger.LogError("Could not find module [WarpsModule]!");
            return;
        }

        warpsModule.SendWarps(caller);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "warps";
    public string Help => "Lists all warps available for a user.";
    public string Syntax => "warps";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}