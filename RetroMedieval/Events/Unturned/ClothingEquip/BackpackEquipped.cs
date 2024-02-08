using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class BackpackEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class BackpackEquippedEventPublisher
{
    public delegate void BackpackEquippedEventHandler(BackpackEquippedEventArgs e);

    public static event BackpackEquippedEventHandler BackpackEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        BackpackEquippedEvent?.Invoke(new BackpackEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}