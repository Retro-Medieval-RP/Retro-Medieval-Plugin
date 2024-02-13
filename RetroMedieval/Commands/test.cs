using System.Collections.Generic;
using Pathfinding.Util;
using RetroMedieval.Models.Kits;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Kits;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Commands;

public class Test : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            Logger.Log("Error: " + Syntax);
            return;
        }

        var kit = new Kit
        {
            KitName = command[0],
            Cooldown = int.Parse(command[1]),
            KitID = Guid.NewGuid()
        };

        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var module) || module == null)
        {
            Logger.Log("Error: Could not gather KitsModule or module was null");
            return;
        }
        
        module?.CreateKit(kit);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Console;
    public string Name => "send";
    public string Help => "send <kit name> <cooldown>";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}