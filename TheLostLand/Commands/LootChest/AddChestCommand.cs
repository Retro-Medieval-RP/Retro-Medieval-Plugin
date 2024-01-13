using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Modules.Loot_Chests;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.LootChest;

public class AddChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "A Syntax Error Has Occured: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<LootChestModule>(out var loot_chest))
        {
            Logger.LogError("Could not find or get module [LootChestModule]!");
            return;
        }
        
        loot_chest.AddChestLocation(((UnturnedPlayer)caller).Position, command[0]);
        Logger.Log($"Added chest location {((UnturnedPlayer)caller).Position.x} {((UnturnedPlayer)caller).Position.y} {((UnturnedPlayer)caller).Position.z}");
        UnturnedChat.Say(caller, $"Added chest location at  {((UnturnedPlayer)caller).Position.x} {((UnturnedPlayer)caller).Position.y} {((UnturnedPlayer)caller).Position.z}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addchest";
    public string Help => "Adds a loot chest at location of player calling command.";
    public string Syntax => "/addchest <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}