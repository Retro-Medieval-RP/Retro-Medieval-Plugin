using System.Collections.Generic;
using System.Linq;
using TheLostLand.Models.LootChest;
using TheLostLand.Savers;
using UnityEngine;

namespace TheLostLand.Modules.LootChest;

internal class LootChestLocationStorage : JsonSaver<List<ChestLocation>>
{
    public ChestLocation GetLocations(string zone_name) => 
        StorageItem.First(x => x.ZoneName == zone_name);

    public void AddLocation(string zone_name, Vector3 position, Quaternion rotation)
    {
        var location = new Location
        {
            X = position.x,
            Y = position.y,
            Z = position.z,
            RotX = rotation.x,
            RotY = rotation.y,
            RotZ = rotation.z,
            RotW = rotation.w
        };
        
        if (StorageItem.Exists(x => x.ZoneName == zone_name))
        {
            var loc = GetLocations(zone_name);
            loc.Locations.Add(location);
            
            Save();
            return;
        }
        
        StorageItem.Add(new ChestLocation(zone_name, location));
        Save();
    }

    public void RemoveLocation(string zone_name, int id)
    {
        var locations = GetLocations(zone_name);
        locations.Locations.RemoveAt(id);

        if (locations.Locations.Count == 0)
        {
            StorageItem.RemoveFast(locations);
        }
        
        Save();
    }
}