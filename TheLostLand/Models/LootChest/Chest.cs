namespace TheLostLand.Models.LootChest;

internal class Chest(double spawn_chance, ChestRarity rarity)
{
    public double ChestSpawnChance { get; set; } = spawn_chance;
    public ChestRarity Rarity { get; set; } = rarity;
}