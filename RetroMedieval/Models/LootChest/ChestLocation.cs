using System.Collections.Generic;
using System.Linq;

namespace RetroMedieval.Models.LootChest;

internal class ChestLocation
{
    public ChestLocation()
    {
    }
    
    public ChestLocation(string zone_name, params Location[] locations)
    {
        ZoneName = zone_name;
        Locations = locations.ToList();
    }

    public string ZoneName { get; set; }
    public List<Location> Locations { get; set; }
    public List<LootChestFlags> Flags { get; set; }
}