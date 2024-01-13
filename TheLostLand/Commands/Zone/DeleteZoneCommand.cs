using Rocket.Unturned.Chat;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zone;

public class DeleteZoneCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "A Syntax Error Has Occured: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones))
        {
            Logger.LogError("Could not find or get module [ZonesModule]!");
            return;
        }
        
        zones.DeleteZone(command[0]);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletezone";
    public string Help => "";
    public string Syntax => "/deletezone <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}