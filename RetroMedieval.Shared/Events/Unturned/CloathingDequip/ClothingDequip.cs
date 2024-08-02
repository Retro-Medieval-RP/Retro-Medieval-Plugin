using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class ClothingDequipEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class ClothingDequipEventPublisher
{
    public delegate void ClothingDequipEventHandler(ClothingDequipEventArgs e, ref bool allow);

    public static event ClothingDequipEventHandler ClothingDequipEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        ClothingDequipEvent?.Invoke(new ClothingDequipEventArgs
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}