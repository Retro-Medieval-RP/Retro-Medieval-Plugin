using Rocket.Unturned.Player;
using TheLostLand.Models.Zones;

namespace TheLostLand.Events.Zones
{
    internal class ZoneLeftEventArgs(UnturnedPlayer player, Zone zone)
    {
        internal UnturnedPlayer Player { get; set; } = player;
        internal Zone Zone { get; set; } = zone;
    }

    internal static class ZoneLeftEventPublisher
    {
        public delegate void ZoneLeftEventHandler(ZoneLeftEventArgs e);

        public static event ZoneLeftEventHandler ZoneLeftEvent;

        internal static void RaiseEvent(UnturnedPlayer player, Zone zone) =>
            ZoneLeftEvent?.Invoke(new ZoneLeftEventArgs(player, zone));
    }
}