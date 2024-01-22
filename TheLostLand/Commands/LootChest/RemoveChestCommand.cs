using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands.LootChest;

internal class RemoveChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "removechest";
    public string Help => "Removes a chest at an ID from a Zone.";
    public string Syntax => "removechest <zone name> <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}