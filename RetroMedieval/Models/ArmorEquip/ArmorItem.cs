namespace RetroMedieval.Models.ArmorEquip;

internal class ArmorItem
{
    public ushort Item;
    public byte Amount;
    public byte Quality;
    public byte[] State;

    public ArmorItem()
    {
    }

    public ArmorItem(ushort item, byte amount, byte quality, byte[] state)
    {
        Item = item;
        Amount = amount;
        Quality = quality;
        State = state;
    }
}