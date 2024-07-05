using RetroMedieval.Modules;
using Rocket.Core.Logging;
using Zones;
using Zones.Models;

namespace LootChest.Events;

public class RemoveLootChestEventArgs
{
    public string ZoneName { get; set; }

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

public static class RemoveLootChestEventPublisher
{
    public delegate void RemoveLootChestEventHandler(RemoveLootChestEventArgs e);

    public static event RemoveLootChestEventHandler RemoveLootChestEvent;

    internal static void RaiseEvent(string zoneName) =>
        RemoveLootChestEvent?.Invoke(new RemoveLootChestEventArgs
        {
            ZoneName = zoneName
        });
}