using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.CloathingDequip;

public class MaskDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class MaskDequippedEventPublisher
{
    public delegate void MaskDequippedEventHandler(MaskDequippedEventArgs e);

    public static event MaskDequippedEventHandler MaskDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id) =>
        MaskDequippedEvent?.Invoke(new MaskDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        });
}