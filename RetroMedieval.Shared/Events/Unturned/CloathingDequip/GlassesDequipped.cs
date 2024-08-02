using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class GlassesDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class GlassesDequippedEventPublisher
{
    public delegate void GlassesDequippedEventHandler(GlassesDequippedEventArgs e, ref bool allow);

    public static event GlassesDequippedEventHandler GlassesDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id, ref bool allow) =>
        GlassesDequippedEvent?.Invoke(new GlassesDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        }, ref allow);
}