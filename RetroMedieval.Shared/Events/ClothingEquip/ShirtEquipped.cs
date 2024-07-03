using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.ClothingEquip;

public class ShirtEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class ShirtEquippedEventPublisher
{
    public delegate void ShirtEquippedEventHandler(ShirtEquippedEventArgs e);

    public static event ShirtEquippedEventHandler ShirtEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        ShirtEquippedEvent?.Invoke(new ShirtEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}