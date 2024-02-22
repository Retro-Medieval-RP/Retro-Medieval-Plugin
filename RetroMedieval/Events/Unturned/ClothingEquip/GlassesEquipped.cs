using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class GlassesEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class GlassesEquippedEventPublisher
{
    public delegate void GlassesEquippedEventHandler(GlassesEquippedEventArgs e);

    public static event GlassesEquippedEventHandler GlassesEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        GlassesEquippedEvent?.Invoke(new GlassesEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}