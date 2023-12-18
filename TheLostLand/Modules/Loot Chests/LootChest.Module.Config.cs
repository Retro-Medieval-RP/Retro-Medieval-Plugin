using TheLostLand.Configs;
using TheLostLand.Models;
using TheLostLand.Models.Loot_Chests;

namespace TheLostLand.Modules.Loot_Chests;

public partial class LootChestModule : IConfig
{
    public List<LootChest> LootChests = [];
    
    public void LoadDefaults()
    {
        LootChests =
        [
            new LootChest(63004, 0, ChestTier.Unique, []),
            new LootChest(63003, 0, ChestTier.Legendary, []),
            new LootChest(63002, 0, ChestTier.Rare, []),
            new LootChest(63001, 0, ChestTier.Uncommon, []),
            new LootChest(63000, 0, ChestTier.Common, [])
        ];
    }
}
