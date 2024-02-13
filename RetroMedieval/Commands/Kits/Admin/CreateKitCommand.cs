using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Kits.Admin;

internal class CreateKitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "createkit";
    public string Help => "Creates a kit from the items you are currently wearing.";
    public string Syntax => "createkit <kit name> [cooldown]";
    public List<string> Aliases => ["ckit"];
    public List<string> Permissions => [];
}