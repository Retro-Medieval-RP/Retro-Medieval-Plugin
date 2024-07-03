namespace RetroMedieval.Models.LootChest;

internal class LootItem
{
    public LootItem()
    {
    }
    
    public LootItem(ushort itemID, double spawnChance)
    {
        SpawnChance = spawnChance;
        LootItemID = itemID;
    }

    public double SpawnChance { get; set; }
    public ushort LootItemID { get; set; }
}