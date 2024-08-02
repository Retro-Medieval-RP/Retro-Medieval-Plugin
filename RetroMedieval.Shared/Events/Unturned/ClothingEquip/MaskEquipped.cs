using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class MaskEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class MaskEquippedEventPublisher
{
    public delegate void MaskEquippedEventHandler(MaskEquippedEventArgs e, ref bool allow);

    public static event MaskEquippedEventHandler MaskEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item, ref bool allow) =>
        MaskEquippedEvent?.Invoke(new MaskEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        }, ref allow);
}