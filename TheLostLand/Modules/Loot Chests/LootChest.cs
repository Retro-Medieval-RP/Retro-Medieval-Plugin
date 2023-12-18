namespace TheLostLand.Modules.Loot_Chests;

public class LootChest(ushort chest_id, ushort effect_id, ChestTier chest_tier)
{
    public ushort ChestID { get; set; } = chest_id;
    public ushort EffectID { get; set; } = effect_id;
    public ChestTier ChestTier { get; set; } = chest_tier;
}