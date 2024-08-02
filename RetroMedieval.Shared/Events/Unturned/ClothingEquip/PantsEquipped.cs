using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class PantsEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class PantsEquippedEventPublisher
{
    public delegate void PantsEquippedEventHandler(PantsEquippedEventArgs e, ref bool allow);

    public static event PantsEquippedEventHandler PantsEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item, ref bool allow) =>
        PantsEquippedEvent?.Invoke(new PantsEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        }, ref allow);
}