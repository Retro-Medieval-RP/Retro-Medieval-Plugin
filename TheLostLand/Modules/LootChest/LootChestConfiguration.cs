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
            new Chest(0.5, ChestRarity.Common, 63000, 5, 
            [
                new LootItem(20, 0.3),
                new LootItem(18, 0.1),
                new LootItem(14, 0.8),
                new LootItem(16, 0.4),
                new LootItem(15, 0.4)
            ]),
            new Chest(0.4, ChestRarity.Uncommon, 63001, 7,
            [
                new LootItem(20, 0.3),
                new LootItem(18, 0.1),
                new LootItem(14, 0.8),
                new LootItem(16, 0.4),
                new LootItem(15, 0.4)
            ]),
            new Chest(0.08, ChestRarity.Rare, 63002, 10,
            [
                new LootItem(20, 0.3),
                new LootItem(18, 0.1),
                new LootItem(14, 0.8),
                new LootItem(16, 0.4),
                new LootItem(15, 0.4)
            ]),
            new Chest(0.02, ChestRarity.Legendary, 63003, 12,
            [
                new LootItem(20, 0.3),
                new LootItem(18, 0.1),
                new LootItem(14, 0.8),
                new LootItem(16, 0.4),
                new LootItem(15, 0.4)
            ])
        ];
    }
}