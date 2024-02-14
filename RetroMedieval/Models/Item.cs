using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models;

public class Item
{
    [DatabaseColumn("ItemID", "SMALLINT UNSIGNED")]
    public ushort ItemID { get; set; }

    [DatabaseColumn("ItemAmount", "INT")]
    public byte Amount { get; set; }

    [DatabaseColumn("ItemQuality", "TINYINT UNSIGNED")]
    public byte Quality { get; set; }

    [DatabaseColumn("ItemState", "BLOB")]
    public byte[] State { get; set; }

    protected Item()
    {
    }

    public Item(ushort item, byte amount, byte quality, byte[] state)
    {
        ItemID = item;
        Amount = amount;
        Quality = quality;
        State = state;
    }
}