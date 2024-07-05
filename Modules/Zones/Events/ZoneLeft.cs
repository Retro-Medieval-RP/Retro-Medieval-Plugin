using Rocket.Unturned.Player;
using Zones.Models;

namespace Zones.Events;

public class ZoneLeftEventArgs(UnturnedPlayer player, Zone zone)
{
    public UnturnedPlayer Player { get; set; } = player;
    public Zone Zone { get; set; } = zone;
}

public static class ZoneLeftEventPublisher
{
    public delegate void ZoneLeftEventHandler(ZoneLeftEventArgs e);

    public static event ZoneLeftEventHandler ZoneLeftEvent;

    internal static void RaiseEvent(UnturnedPlayer player, Zone zone) =>
        ZoneLeftEvent?.Invoke(new ZoneLeftEventArgs(player, zone));
}