using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Kits.Admin;

internal class RenameKitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "renamekit";
    public string Help => "Renames a kit.";
    public string Syntax => "renamekit <current kit name> <new kit name>";
    public List<string> Aliases => [ "rkit" ];
    public List<string> Permissions => [];
}