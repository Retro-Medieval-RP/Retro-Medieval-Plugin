using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.CloathingDequip;

public class ShirtDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class ShirtDequippedEventPublisher
{
    public delegate void ShirtDequippedEventHandler(ShirtDequippedEventArgs e);

    public static event ShirtDequippedEventHandler ShirtDequippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort id) =>
        ShirtDequippedEvent?.Invoke(new ShirtDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        });
}