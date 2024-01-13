namespace TheLostLand.Commands.Zone.Nodes;

public class AddNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "addnode";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}