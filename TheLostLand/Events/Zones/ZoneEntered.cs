using Steamworks;
using TheLostLand.Models.Zones;

namespace TheLostLand.Events.Zones
{
    internal class ZoneEnteredEventArgs(CSteamID user_id, Zone zone)
    {
        internal CSteamID UserId { get; set; } = user_id;
        internal Zone Zone { get; set; } = zone;
    }

    internal class ZoneEnteredEventPublisher
    {
        public delegate void ZoneEnteredEventHandler(ZoneEnteredEventArgs e);

        public static event ZoneEnteredEventHandler ZoneEnteredEvent;

        internal static void RaiseEvent(CSteamID user_id, Zone zone) => 
            ZoneEnteredEvent?.Invoke(new ZoneEnteredEventArgs(user_id, zone));
    }
}

