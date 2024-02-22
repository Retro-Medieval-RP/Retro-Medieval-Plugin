using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class MaskEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class MaskEquippedEventPublisher
{
    public delegate void MaskEquippedEventHandler(MaskEquippedEventArgs e);

    public static event MaskEquippedEventHandler MaskEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        MaskEquippedEvent?.Invoke(new MaskEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}