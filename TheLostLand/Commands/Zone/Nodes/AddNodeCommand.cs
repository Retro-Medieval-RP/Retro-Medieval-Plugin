using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zone.Nodes;

public class AddNodeCommand : IRocketCommand
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

        zones.AddNode(command[0], ((UnturnedPlayer)caller).Position);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addnode";
    public string Help => "";
    public string Syntax => "/addnode <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}