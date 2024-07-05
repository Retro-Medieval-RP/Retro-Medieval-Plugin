using System.Collections.Generic;
using System.Linq;

namespace LootChest.Models;

internal class ChestLocation
{
    public ChestLocation()
    {
    }
    
    public ChestLocation(string zoneName, params Location[] locations)
    {
        ZoneName = zoneName;
        Locations = locations.ToList();
    }

    public string ZoneName { get; set; }
    public List<Location> Locations { get; set; }
    public List<LootChestFlags> Flags { get; set; }
}