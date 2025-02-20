using LootChest.Models;
using RetroMedieval.Modules;
using RetroMedieval.Shared.Models.Zones;
using Rocket.Core.Logging;
using Zones;

namespace LootChest.Events;

public class SpawnLootChestEventArgs
{
    public string ZoneName { get; set; }
    public LootChestFlags Flag { get; set; }

    public bool Zone(out Zone zone)
    {
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zonesModule))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            zone = default;
            return false;
        }

        if (zonesModule.Exists(ZoneName))
        {
            return zonesModule.GetZone(ZoneName, out zone);
        }

        Logger.LogError($"Zone {ZoneName} does not exist!");
        zone = default;
        return false;
    }
}

public static class SpawnLootChestEventPublisher
{
    public delegate void SpawnLootChestEventHandler(SpawnLootChestEventArgs e);

    public static event SpawnLootChestEventHandler SpawnLootChestEvent;

    internal static void RaiseEvent(string zoneName, LootChestFlags flag) =>
        SpawnLootChestEvent?.Invoke(new SpawnLootChestEventArgs
        {
            ZoneName = zoneName,
            Flag = flag
        });
}