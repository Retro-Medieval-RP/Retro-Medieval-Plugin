using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class BackpackEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class BackpackEquippedEventPublisher
{
    public delegate void BackpackEquippedEventHandler(BackpackEquippedEventArgs e, ref bool allow);

    public static event BackpackEquippedEventHandler BackpackEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item, ref bool allow) =>
        BackpackEquippedEvent?.Invoke(new BackpackEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        }, ref allow);
}