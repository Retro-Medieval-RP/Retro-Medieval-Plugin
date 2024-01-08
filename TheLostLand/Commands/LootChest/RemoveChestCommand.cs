using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Modules;
using TheLostLand.Modules.Loot_Chests;

namespace TheLostLand.Commands.LootChest;

public class RemoveChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var loot_chest))
        {
            Logger.LogError("Could not find or get module [LootChestModule]!");
            return;
        }
        
        loot_chest.RemoveChestLocation(((UnturnedPlayer)caller).Position);
        Logger.Log($"Removed chest location {((UnturnedPlayer)caller).Position.x} {((UnturnedPlayer)caller).Position.y} {((UnturnedPlayer)caller).Position.z}");
        UnturnedChat.Say($"Removed chest location at  {((UnturnedPlayer)caller).Position.x} {((UnturnedPlayer)caller).Position.y} {((UnturnedPlayer)caller).Position.z}");
    }

    public AllowedCaller AllowedCaller { get; }
    public string Name { get; }
    public string Help { get; }
    public string Syntax { get; }
    public List<string> Aliases { get; }
    public List<string> Permissions { get; }
}