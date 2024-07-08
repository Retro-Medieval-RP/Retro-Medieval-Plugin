using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.ClothingEquip;

public class VestEquippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class VestEquippedEventPublisher
{
    public delegate void VestEquippedEventHandler(VestEquippedEventArgs e);

    public static event VestEquippedEventHandler VestEquippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        VestEquippedEvent?.Invoke(new VestEquippedEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}