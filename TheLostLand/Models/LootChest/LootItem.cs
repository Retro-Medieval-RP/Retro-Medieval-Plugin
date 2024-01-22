namespace TheLostLand.Models.LootChest;

internal class LootItem(ushort item_id, double spawn_chance)
{
    public double SpawnChance { get; set; } = spawn_chance;
    public ushort LootItemID { get; set; } = item_id;
}