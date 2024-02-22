using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Kits;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace RetroMedieval.Commands.Kits.User;

internal class KitsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kits_module))
        {
            Logger.LogError("Could not find module [KitsModule]!");
            return;   
        }

        kits_module.SendKits(caller);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kits";
    public string Help => "Displays a list of all kits available to the user.";
    public string Syntax => "kits";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}