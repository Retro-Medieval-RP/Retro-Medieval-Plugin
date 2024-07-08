using RetroMedieval.Shared.Models.Zones;
using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Zones;

public class ZoneLeftEventArgs(UnturnedPlayer player, Zone zone)
{
    public UnturnedPlayer Player { get; set; } = player;
    public Zone Zone { get; set; } = zone;
}

public static class ZoneLeftEventPublisher
{
    public delegate void ZoneLeftEventHandler(ZoneLeftEventArgs e);

    public static event ZoneLeftEventHandler ZoneLeftEvent;

    public static void RaiseEvent(UnturnedPlayer player, Zone zone) =>
        ZoneLeftEvent?.Invoke(new ZoneLeftEventArgs(player, zone));
}