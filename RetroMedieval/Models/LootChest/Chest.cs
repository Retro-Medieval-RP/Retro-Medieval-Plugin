using System.Collections.Generic;
using System.Linq;

namespace RetroMedieval.Models.LootChest;

internal class Chest
{
    public Chest()
    {
    }

    public Chest(double spawnChance, ChestRarity rarity, ushort chestID, int amountToPick, params LootItem[] spawnTable)
    {
        ChestSpawnChance = spawnChance;
        Rarity = rarity;
        ChestBarricade = chestID;
        AmountToPick = amountToPick;
        SpawnTable = spawnTable.ToList();
    }

    public double ChestSpawnChance { get; set; }
    public ChestRarity Rarity { get; set; }
    public ushort ChestBarricade { get; set; }

    public int AmountToPick { get; set; }
    
    public List<LootItem> SpawnTable { get; set; }
}