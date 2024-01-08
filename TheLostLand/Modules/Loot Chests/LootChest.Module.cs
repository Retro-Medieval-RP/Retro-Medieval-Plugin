using UnityEngine;

namespace TheLostLand.Modules.Loot_Chests;

[ModuleInformation("Loot Module")]
[ModuleStorage<LootChestLocationStorage>("LootChestLocation")]
[ModuleConfiguration<LootChestModuleConfig>("LootChestConfiguration")]
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

    public void AddChestLocation(Vector3 location)
    {
        if(GetStorage<LootChestLocationStorage>(out var storage))
        {
            storage.AddLocation(location);
        }
    }

    public void RemoveChestLocation(Vector3 position)
    {
        if(GetStorage<LootChestLocationStorage>(out var storage))
        {
            storage.RemoveLocation(position);
        }
    }
}