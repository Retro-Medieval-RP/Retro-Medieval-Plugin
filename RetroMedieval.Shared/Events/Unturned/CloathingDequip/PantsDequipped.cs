using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class PantsDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class PantsDequippedEventPublisher
{
    public delegate void PantsDequippedEventHandler(PantsDequippedEventArgs e, ref bool allow);

    public static event PantsDequippedEventHandler PantsDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        PantsDequippedEvent?.Invoke(new PantsDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}