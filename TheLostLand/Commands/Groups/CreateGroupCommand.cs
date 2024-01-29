using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands.Groups;

public class CreateGroupCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        throw new System.NotImplementedException();
    }

    public AllowedCaller AllowedCaller { get; }
    public string Name { get; }
    public string Help { get; }
    public string Syntax { get; }
    public List<string> Aliases { get; }
    public List<string> Permissions { get; }
}