using System.Collections.Generic;
using System.Linq;
using TheLostLand.Core.Modules;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Models.Zones;
using UnityEngine;

namespace TheLostLand.Modules.Zones;

[ModuleInformation("Zones")]
[ModuleStorage<ZonesStorage>("ZonesList")]
public class ZonesModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    private bool IsInZone(Vector3 point) => 
        GetStorage<ZonesStorage>(out var storage) && storage.GetZones().Select(zone => zone.IsInZone(point)).FirstOrDefault();
}