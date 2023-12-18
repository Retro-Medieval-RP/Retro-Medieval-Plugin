namespace TheLostLand.Models.Loot_Chests;

public class LootChest(ushort chest_id, ushort effect_id, ChestTier chest_tier, double chest_chance,  List<LootItem> items)
{
    public ushort ChestID { get; set; } = chest_id;
    public ushort EffectID { get; set; } = effect_id;
    public ChestTier ChestTier { get; set; } = chest_tier;

    public double ChestPlaceChance { get; set; } = chest_chance;

    public List<LootItem> LootItems { get; set; } = items;
}