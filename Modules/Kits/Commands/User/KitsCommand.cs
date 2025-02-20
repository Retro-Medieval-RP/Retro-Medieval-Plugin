using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Core.Logging;

namespace Kits.Commands.User;

internal class KitsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kitsModule))
        {
            Logger.LogError("Could not find module [KitsModule]!");
            return;   
        }

        kitsModule.SendKits(caller);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kits";
    public string Help => "Displays a list of all kits available to the user.";
    public string Syntax => "kits";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}