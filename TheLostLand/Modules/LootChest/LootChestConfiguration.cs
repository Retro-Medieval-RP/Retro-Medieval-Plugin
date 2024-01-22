using System.Collections.Generic;
using TheLostLand.Core.Modules.Configuration;
using TheLostLand.Models.LootChest;

namespace TheLostLand.Modules.LootChest;

internal class LootChestConfiguration : IConfig
{
    public List<Chest> Chests { get; set; }
    
    public void LoadDefaults()
    {
        Chests = [
            new Chest(0.5, ChestRarity.Common),
            new Chest(0.4, ChestRarity.Uncommon),
            new Chest(0.08, ChestRarity.Rare),
            new Chest(0.02, ChestRarity.Legendary)
        ];
    }
}