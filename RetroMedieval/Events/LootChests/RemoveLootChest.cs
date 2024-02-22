using RetroMedieval.Models.Zones;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Zones;
using Rocket.Core.Logging;

namespace RetroMedieval.Events.LootChests;

public class RemoveLootChestEventArgs
{
    public string ZoneName { get; set; }

    public bool Zone(out Zone zone)
    {
        if (!ModuleLoader.Instance.GetModule<ZonesModule>(out var zones_module))
        {
            Logger.LogError("Could not find module [ZonesModule]!");
            zone = default;
            return false;
        }

        if (zones_module.Exists(ZoneName))
        {
            return zones_module.GetZone(ZoneName, out zone);
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

    internal static void RaiseEvent(string zone_name) =>
        RemoveLootChestEvent?.Invoke(new RemoveLootChestEventArgs
        {
            ZoneName = zone_name
        });
}