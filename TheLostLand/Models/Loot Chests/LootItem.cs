namespace TheLostLand.Models.Loot_Chests;

public class LootItem(ushort item_id, double spawn_chance)
{
    public double SpawnChance { get; set; } = spawn_chance;
    public ushort ItemID { get; set; } = item_id;
}