using TheLostLand.Models.Zones;

namespace TheLostLand.Events.LootChests
{
    internal class LootChestSpawnedEventArgs(Zone zone)
    {
        internal Zone LootChestZone { get; set; } = zone;
    }

    internal static class LootChestSpawnedEventPublisher
    {
        public delegate void LootChestSpawnedEventHandler(LootChestSpawnedEventArgs e);

        public static event LootChestSpawnedEventHandler LootChestSpawnedEvent;

        internal static void RaiseEvent(Zone zone) =>
            LootChestSpawnedEvent?.Invoke(new LootChestSpawnedEventArgs(zone));
    }
}