using System.Collections.Generic;
using System.Linq;

namespace TheLostLand.Models.LootChest;

internal class Chest(double spawn_chance, ChestRarity rarity, ushort chest_id, params LootItem[] spawn_table)
{
    public double ChestSpawnChance { get; set; } = spawn_chance;
    public ChestRarity Rarity { get; set; } = rarity;
    public ushort ChestBarricade { get; set; } = chest_id;

    public List<LootItem> SpawnTable { get; set; } = spawn_table.ToList();
}