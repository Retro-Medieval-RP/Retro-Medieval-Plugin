namespace DeadBodies.Models;

public class Item
{
    public ushort ItemID { get; set; }
    public byte Amount { get; set; }
    public byte Quality { get; set; }
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