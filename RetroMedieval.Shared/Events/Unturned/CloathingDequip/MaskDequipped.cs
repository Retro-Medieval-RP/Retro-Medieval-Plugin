using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class MaskDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class MaskDequippedEventPublisher
{
    public delegate void MaskDequippedEventHandler(MaskDequippedEventArgs e, ref bool allow);

    public static event MaskDequippedEventHandler MaskDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        MaskDequippedEvent?.Invoke(new MaskDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}