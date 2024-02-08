namespace RetroMedieval.Models.DeadBodys;

public class DeathItem
{
    public ushort Item;
    public byte Amount;
    public byte Quality;
    public byte[] State;

    public DeathItem()
    {
    }

    public DeathItem(ushort item, byte amount, byte quality, byte[] state)
    {
        Item = item;
        Amount = amount;
        Quality = quality;
        State = state;
    }
}