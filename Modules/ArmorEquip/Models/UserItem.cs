namespace ArmorEquip.Models;

internal class UserItem
{
    public ushort ItemID { get; set; }
    public byte ItemAmount { get; set; }
    public byte[] ItemState { get; set; }
    public byte ItemQuality { get; set; } 
}