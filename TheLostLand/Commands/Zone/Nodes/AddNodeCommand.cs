using Rocket.Core.Logging;
using TheLostLand.Modules.Zones;

namespace TheLostLand.Commands.Zone.Nodes;

public class AddNodeCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones))
        {
            Logger.LogError("Could not find or get module [ZonesModule]!");
            return;
        }
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "addnode";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}