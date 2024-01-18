using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands;

internal class AddNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addnode";
    public string Help => "Adds a node to a zone.";
    public string Syntax => "addnode <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}