using TheLostLand.Core.Storage;
using UnityEngine;

namespace TheLostLand.Modules.Loot_Chests;

public class LootChestLocationStorage : IStorage
{
    private List<LootChestLocation> Locations { get; set; } = [];
    public void Save()
    {
    }

    public void Load()
    {
    }

    public void AddLocation(Vector3 location, string zone)
    {
        Locations.Add(new LootChestLocation
        {
            X = location.x,
            Y = location.y,
            Z = location.z,
            ZoneName = zone
        });
        Save();
    }

    public void RemoveLocation(Vector3 location)
    {
        Locations.RemoveAll(loc => location.Equals(new Vector3(loc.X, loc.Y, loc.Z)));
        Save();
    }

    public IEnumerable<LootChestLocation> GetLocationsFromZone(string zone) => 
        Locations.Where(x => x.ZoneName == zone);

    public bool AnyChestsInZone(string zone_name) => 
        Locations.Any(x => x.ZoneName == zone_name);
}