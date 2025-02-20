using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class HatDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class HatDequippedEventPublisher
{
    public delegate void HatDequippedEventHandler(HatDequippedEventArgs e, ref bool allow);

    public static event HatDequippedEventHandler HatDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        HatDequippedEvent?.Invoke(new HatDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}