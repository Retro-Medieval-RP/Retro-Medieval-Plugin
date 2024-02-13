using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Kits.User;

internal class KitsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kits";
    public string Help => "Displays a list of all kits available to the user.";
    public string Syntax => "kits [page num]";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}