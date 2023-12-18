using TheLostLand.Configs;

namespace TheLostLand.Modules.Loot_Chests;

public partial class LootChestModule : ModuleConfiguration
{
    public List<LootChest> LootChests = [];
    
    public override void LoadDefaults()
    {
        LootChests =
        [
            new LootChest(63004, 0, ChestTier.Unique),
            new LootChest(63003, 0, ChestTier.Legendary),
            new LootChest(63002, 0, ChestTier.Rare),
            new LootChest(63001, 0, ChestTier.Uncommon),
            new LootChest(63000, 0, ChestTier.Common)
        ];
    }
}
