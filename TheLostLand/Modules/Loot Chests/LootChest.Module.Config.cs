namespace TheLostLand.Modules.Loot_Chests;

[ModuleConfiguration<LootChestModuleConfig>("LootChestConfig", "Loot Module")]
public class LootChestModuleConfig : IConfig
{
    public List<LootChest> LootChests = [];
    
    public void LoadDefaults()
    {
        LootChests =
        [
            new LootChest(63004, 0, ChestTier.Unique, 0.05, []),
            new LootChest(63003, 0, ChestTier.Legendary, 0.1, []),
            new LootChest(63002, 0, ChestTier.Rare, 0.2, []),
            new LootChest(63001, 0, ChestTier.Uncommon, 0.3, []),
            new LootChest(63000, 0, ChestTier.Common, 0.35, [])
        ];
    }
}
