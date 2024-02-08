using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.ClothingEquip;

public class HatEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class HatEquippedEventPublisher
{
    public delegate void HatEquippedEventHandler(HatEquippedEventArgs e);

    public static event HatEquippedEventHandler HatEquippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        HatEquippedEvent?.Invoke(new HatEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}