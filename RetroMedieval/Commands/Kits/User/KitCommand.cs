using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Kits.User;

internal class KitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kit";
    public string Help => "Spawns a kit for a user.";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];

    public string AdminSyntax => "kit <player name | player id> <kit name>";
    public string UserSyntax => "kit <kit name>";
}