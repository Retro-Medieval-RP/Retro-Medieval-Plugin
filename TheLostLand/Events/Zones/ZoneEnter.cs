using Rocket.Unturned.Player;
using TheLostLand.Models.Zones;

namespace TheLostLand.Events.Zones
{
    internal class ZoneEnterEventArgs(UnturnedPlayer player, Zone zone)
    {
        internal UnturnedPlayer Player { get; set; } = player;
        internal Zone Zone { get; set; } = zone;
    }

    internal static class ZoneEnterEventPublisher
    {
        public delegate void ZoneEnterEventHandler(ZoneEnterEventArgs e);

        public static event ZoneEnterEventHandler ZoneEnterEvent;

        internal static void RaiseEvent(UnturnedPlayer player, Zone zone) =>
            ZoneEnterEvent?.Invoke(new ZoneEnterEventArgs(player, zone));
    }
}