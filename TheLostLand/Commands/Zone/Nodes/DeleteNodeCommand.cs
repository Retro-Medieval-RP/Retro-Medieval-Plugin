using Rocket.Unturned.Chat;
using TheLostLand.Modules.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Zone.Nodes;

public class DeleteNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say("A Syntax Error Has Occured: ", Color.red);
            UnturnedChat.Say(Syntax, Color.red);
            return;
        }

        if (!int.TryParse(command[1], out var id))
        {
            UnturnedChat.Say("A Syntax Error Has Occured | Could not parse ID: ", Color.red);
            UnturnedChat.Say(Syntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones))
        {
            Logger.LogError("Could not find or get module [ZonesModule]!");
            return;
        }

        zones.DeleteNode(command[0], id);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "deletenode";
    public string Help => "";
    public string Syntax => "/deletenode <zone name> <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}