using System.Collections.Generic;
using System.Linq;

namespace TheLostLand.Models.LootChest;

internal class ChestLocation(string zone_name, params Location[] locations)
{
    public string ZoneName { get; set; } = zone_name;
    public List<Location> Locations { get; set; } = locations.ToList();
}