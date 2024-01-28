using System.Collections.Generic;
using System.Linq;

namespace TheLostLand.Models.LootChest;

internal class Chest
{
    public Chest()
    {
    }

    public Chest(double spawn_chance, ChestRarity rarity, ushort chest_id, int amount_to_pick, params LootItem[] spawn_table)
    {
        ChestSpawnChance = spawn_chance;
        Rarity = rarity;
        ChestBarricade = chest_id;
        AmountToPick = amount_to_pick;
        SpawnTable = spawn_table.ToList();
    }

    public double ChestSpawnChance { get; set; }
    public ChestRarity Rarity { get; set; }
    public ushort ChestBarricade { get; set; }

    public int AmountToPick { get; set; }
    
    public List<LootItem> SpawnTable { get; set; }
}