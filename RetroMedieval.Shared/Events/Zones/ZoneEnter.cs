using RetroMedieval.Shared.Models.Zones;
using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Zones;

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

    public static void RaiseEvent(ref UnturnedPlayer player, ref Zone zone,ref bool allow) =>
        ZoneEnterEvent?.Invoke(new ZoneEnterEventArgs(ref player, ref zone, ref allow));
}