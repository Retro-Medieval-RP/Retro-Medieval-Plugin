using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class PantsEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class PantsEquippedEventPublisher
{
    public delegate void PantsEquippedEventHandler(PantsEquippedEventArgs e);

    public static event PantsEquippedEventHandler PantsEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        PantsEquippedEvent?.Invoke(new PantsEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}