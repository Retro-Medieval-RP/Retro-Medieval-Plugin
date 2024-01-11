using Steamworks;
using TheLostLand.Models.Zones;

namespace TheLostLand.Events.Zones
{
    internal class ZoneLeftEventArgs(CSteamID user_id, Zone zone)
    {
        internal CSteamID UserId { get; set; } = user_id;
        internal Zone Zone { get; set; } = zone;
    }

    internal class ZoneLeftEventPublisher
    {
        public delegate void ZoneLeftEventHandler(ZoneLeftEventArgs e);

        public static event ZoneLeftEventHandler ZoneLeftEvent;

        internal static void RaiseEvent(CSteamID user_id, Zone zone) => 
            ZoneLeftEvent?.Invoke(new ZoneLeftEventArgs(user_id, zone));
    }
}