using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.LootChest;
using RetroMedieval.Savers;
using UnityEngine;

namespace RetroMedieval.Modules.LootChest;

internal class LootChestLocationStorage : JsonSaver<List<ChestLocation>>
{
    public ChestLocation GetLocations(string zone_name) => 
        StorageItem.First(x => x.ZoneName == zone_name);

    public void AddLocation(string zone_name, Vector3 position, Quaternion rotation, List<LootChestFlags> flags_list)
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
        
        StorageItem.Add(new ChestLocation(zone_name, location)
        {
            Flags = flags_list
        });
        Save();
    }

    public bool RemoveLocation(string zone_name, int id)
    {
        var locations = GetLocations(zone_name);

        if (locations.Locations.Count == 0)
        {
            return false;
        }
        
        locations.Locations.RemoveAt(id);
        RemoveLootZone(zone_name);
        
        Save();
        return true;
    }

    private void RemoveLootZone(string zone_name)
    {
        var locations = GetLocations(zone_name);
        
        if (locations.Locations.Count == 0)
        {
            StorageItem.RemoveFast(locations);
        }
    }
}