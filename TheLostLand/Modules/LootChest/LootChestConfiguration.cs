using System.Collections.Generic;
using TheLostLand.Models.LootChest;
using TheLostLand.Modules.Configuration;

namespace TheLostLand.Modules.LootChest;

internal class LootChestConfiguration : IConfig
{
    public List<Chest> Chests { get; set; }
    
    public void LoadDefaults()
    {
        Chests = [
            new Chest(0.5, ChestRarity.Common, 63000, 5),
            new Chest(0.4, ChestRarity.Uncommon, 63001, 7),
            new Chest(0.08, ChestRarity.Rare, 63002, 10),
            new Chest(0.02, ChestRarity.Legendary, 63003, 12)
        ];
    }
}