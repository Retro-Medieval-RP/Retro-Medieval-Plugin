using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands;

internal class RemoveNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "removenode";
    public string Help => "Removes a node from a zone.";
    public string Syntax => "removenode <zone name> <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}