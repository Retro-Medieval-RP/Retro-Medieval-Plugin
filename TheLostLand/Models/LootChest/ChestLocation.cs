using System.Collections.Generic;
using System.Linq;

namespace TheLostLand.Models.LootChest;

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
}