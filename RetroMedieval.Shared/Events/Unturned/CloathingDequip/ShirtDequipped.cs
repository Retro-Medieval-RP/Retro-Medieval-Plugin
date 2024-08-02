using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class ShirtDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class ShirtDequippedEventPublisher
{
    public delegate void ShirtDequippedEventHandler(ShirtDequippedEventArgs e, ref bool allow);

    public static event ShirtDequippedEventHandler ShirtDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        ShirtDequippedEvent?.Invoke(new ShirtDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}