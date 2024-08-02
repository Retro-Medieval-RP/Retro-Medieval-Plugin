using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class BackpackDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class BackpackDequippedEventPublisher
{
    public delegate void BackpackDequippedEventHandler(BackpackDequippedEventArgs e, ref bool allow);

    public static event BackpackDequippedEventHandler BackpackDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        BackpackDequippedEvent?.Invoke(new BackpackDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}