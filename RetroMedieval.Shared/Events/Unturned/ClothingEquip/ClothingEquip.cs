using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class ClothingEquipEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class ClothingEquipEventPublisher
{
    public delegate void ClothingEquipEventHandler(ClothingEquipEventArgs e, ref bool allow);

    public static event ClothingEquipEventHandler ClothingEquipEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item, ref bool allow) =>
        ClothingEquipEvent?.Invoke(new ClothingEquipEventArgs
        {
            Player = player,
            ClothingItem = item
        }, ref allow);
}