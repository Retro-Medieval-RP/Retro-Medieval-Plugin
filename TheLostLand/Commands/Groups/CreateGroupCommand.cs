using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands.Groups;

public class CreateGroupCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "creategroup";
    public string Help => "Creates a group.";
    public string Syntax => "creategroup <group name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}