using Rocket.Unturned.Player;
using TheLostLand.Models.Zones;

namespace TheLostLand.Events.Zones;

public class ZoneEnterEventArgs(ref UnturnedPlayer player, ref Zone zone, ref bool allow)
{
    internal UnturnedPlayer Player { get; set; } = player;
    internal Zone Zone { get; set; } = zone;
    internal bool Allow { get; set; } = allow;
}

public static class ZoneEnterEventPublisher
{
    public delegate void ZoneEnterEventHandler(ZoneEnterEventArgs e);

    public static event ZoneEnterEventHandler ZoneEnterEvent;

    internal static void RaiseEvent(ref UnturnedPlayer player, ref Zone zone,ref bool allow) =>
        ZoneEnterEvent?.Invoke(new ZoneEnterEventArgs(ref player, ref zone, ref allow));
}