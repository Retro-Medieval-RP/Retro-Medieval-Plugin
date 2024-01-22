namespace TheLostLand.Models.LootChest;

internal class Chest(double spawn_chance, ChestRarity rarity, ushort chest_id)
{
    public double ChestSpawnChance { get; set; } = spawn_chance;
    public ChestRarity Rarity { get; set; } = rarity;
    public ushort ChestBarricade { get; set; } = chest_id;
}