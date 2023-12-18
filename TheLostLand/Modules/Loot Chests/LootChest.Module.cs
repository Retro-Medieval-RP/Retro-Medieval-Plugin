using TheLostLand.Models.Loot_Chests;
using TheLostLand.Modules.Attributes;
using TheLostLand.Utils;

namespace TheLostLand.Modules.Loot_Chests;

[ModuleInformation("Loot Module")]
public class LootChestModule : Module
{
    private Picker<LootChest> Picker { get; set; } = new();
    public LootChestModule()
    {
        if (!GetConfiguration<LootChestModuleConfig>(out var config)) return;
        
        foreach (var chest in config.LootChests)
        {
            Picker.AddEntry(chest, chest.ChestPlaceChance);        
        }
    }

    public void SpawnChest()
    {
        
    }
}