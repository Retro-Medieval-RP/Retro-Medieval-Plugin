using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class GlassesEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class GlassesEquippedEventPublisher
{
    public delegate void GlassesEquippedEventHandler(GlassesEquippedEventArgs e, ref bool allow);

    public static event GlassesEquippedEventHandler GlassesEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item, ref bool allow) =>
        GlassesEquippedEvent?.Invoke(new GlassesEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        }, ref allow);
}