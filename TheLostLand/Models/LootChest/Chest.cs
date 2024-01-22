using System.Collections.Generic;
using System.Linq;

namespace TheLostLand.Models.LootChest;

internal class Chest(double spawn_chance, ChestRarity rarity, ushort chest_id, int amount_to_pick, params LootItem[] spawn_table)
{
    public double ChestSpawnChance { get; set; } = spawn_chance;
    public ChestRarity Rarity { get; set; } = rarity;
    public ushort ChestBarricade { get; set; } = chest_id;

    public int AmountToPick { get; set; } = amount_to_pick;
    
    public List<LootItem> SpawnTable { get; set; } = spawn_table.ToList();
}