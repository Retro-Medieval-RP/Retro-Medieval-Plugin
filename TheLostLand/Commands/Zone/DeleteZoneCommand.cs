namespace TheLostLand.Commands.Zone;

public class DeleteZoneCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletezone";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}