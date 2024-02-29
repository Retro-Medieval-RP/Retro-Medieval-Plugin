using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.CloathingDequip;

public class GlassesDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class GlassesDequippedEventPublisher
{
    public delegate void GlassesDequippedEventHandler(GlassesDequippedEventArgs e);

    public static event GlassesDequippedEventHandler GlassesDequippedEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ushort id) =>
        GlassesDequippedEvent?.Invoke(new GlassesDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        });
}