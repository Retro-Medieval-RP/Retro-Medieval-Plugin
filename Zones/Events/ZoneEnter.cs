using Rocket.Unturned.Player;
using Zones.Models;

namespace Zones.Events;

public class ZoneEnterEventArgs(ref UnturnedPlayer player, ref Zone zone, ref bool allow)
{
    public UnturnedPlayer Player { get; set; } = player;
    public Zone Zone { get; set; } = zone;
    public bool Allow { get; set; } = allow;
}

public static class ZoneEnterEventPublisher
{
    public delegate void ZoneEnterEventHandler(ZoneEnterEventArgs e);

    public static event ZoneEnterEventHandler ZoneEnterEvent;

    internal static void RaiseEvent(ref UnturnedPlayer player, ref Zone zone,ref bool allow) =>
        ZoneEnterEvent?.Invoke(new ZoneEnterEventArgs(ref player, ref zone, ref allow));
}