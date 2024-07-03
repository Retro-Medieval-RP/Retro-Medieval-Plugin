using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.LootChest;
using RetroMedieval.Savers.Json;
using UnityEngine;

namespace RetroMedieval.Modules.LootChest;

internal class LootChestLocationStorage : JsonSaver<List<ChestLocation>>
{
    public ChestLocation GetLocations(string zoneName) => 
        StorageItem.First(x => x.ZoneName == zoneName);

    public void AddLocation(string zoneName, Vector3 position, Quaternion rotation, List<LootChestFlags> flagsList)
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
        
        if (StorageItem.Exists(x => x.ZoneName == zoneName))
        {
            var loc = GetLocations(zoneName);
            loc.Locations.Add(location);
            
            Save();
            return;
        }
        
        StorageItem.Add(new ChestLocation(zoneName, location)
        {
            Flags = flagsList
        });
        Save();
    }

    public bool RemoveLocation(string zoneName, int id)
    {
        var locations = GetLocations(zoneName);

        if (locations.Locations.Count == 0)
        {
            return false;
        }
        
        locations.Locations.RemoveAt(id);
        RemoveLootZone(zoneName);
        
        Save();
        return true;
    }

    private void RemoveLootZone(string zoneName)
    {
        var locations = GetLocations(zoneName);
        
        if (locations.Locations.Count == 0)
        {
            StorageItem.RemoveFast(locations);
        }
    }
}