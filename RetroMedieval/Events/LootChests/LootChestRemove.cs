using RetroMedieval.Models.Zones;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Zones;
using Rocket.Core.Logging;

namespace RetroMedieval.Events.LootChests;

public class LootChestRemoveEventArgs
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

public static class LootChestRemoveEventPublisher
{
    public delegate void LootChestRemoveEventHandler(LootChestRemoveEventArgs e);

    public static event LootChestRemoveEventHandler LootChestRemoveEvent;

    internal static void RaiseEvent(string zone_name) =>
        LootChestRemoveEvent?.Invoke(new LootChestRemoveEventArgs
        {
            ZoneName = zone_name
        });
}