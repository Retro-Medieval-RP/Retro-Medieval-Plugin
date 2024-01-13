namespace TheLostLand.Commands.Zone.Nodes;

public class DeleteNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletenode";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}