namespace TheLostLand.Commands.Zone;

public class CreateZoneCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "createzone";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}