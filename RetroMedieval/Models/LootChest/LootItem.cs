namespace RetroMedieval.Models.LootChest;

internal class LootItem
{
    public LootItem()
    {
    }
    
    public LootItem(ushort item_id, double spawn_chance)
    {
        SpawnChance = spawn_chance;
        LootItemID = item_id;
    }

    public double SpawnChance { get; set; }
    public ushort LootItemID { get; set; }
}