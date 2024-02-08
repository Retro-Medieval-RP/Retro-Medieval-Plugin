using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class ShirtEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class ShirtEquippedEventPublisher
{
    public delegate void ShirtEquippedEventHandler(ShirtEquippedEventArgs e);

    public static event ShirtEquippedEventHandler ShirtEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        ShirtEquippedEvent?.Invoke(new ShirtEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}