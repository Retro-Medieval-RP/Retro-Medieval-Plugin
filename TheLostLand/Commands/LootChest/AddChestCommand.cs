using System.Collections.Generic;
using Rocket.API;

namespace TheLostLand.Commands.LootChest;

internal class AddChestCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "addchest";
    public string Help => "Adds a chest at a location and at an angle.";
    public string Syntax => "addchest <zone name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}