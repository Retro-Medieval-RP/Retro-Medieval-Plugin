using Rocket.Unturned.Chat;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zone;

public class CreateZoneCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say("A Syntax Error Has Occured: ", Color.red);
            UnturnedChat.Say(Syntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones))
        {
            Logger.LogError("Could not find or get module [ZonesModule]!");
            return;
        }

        zones.CreateZone(command[0]);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "createzone";
    public string Help => "";
    public string Syntax => "/createzone <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}