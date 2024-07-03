using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Zones;
using RetroMedieval.Savers.Json;

namespace RetroMedieval.Modules.Zones;

public class ZonesStorage : JsonSaver<List<Zone>>
{
    public void AddZone(string zoneName)
    {
        StorageItem.Add(new Zone(zoneName, []));
        Save();
    }

    public void RemoveZone(string zoneName)
    {
        StorageItem.RemoveAll(x => x.ZoneName == zoneName);
        Save();
    }

    public void AddNode(string zoneName, Node node)
    {
        if (StorageItem.All(x => x.ZoneName != zoneName))
        {
            return;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zoneName);
        zone.Nodes.Add(node);

        Save();
    }

    public bool RemoveNode(string zoneName, int id)
    {
        if (StorageItem.All(x => x.ZoneName != zoneName))
        {
            return false;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zoneName);

        if (zone.Nodes.Count == 0)
        {
            return false;
        }
        
        zone.Nodes.RemoveAt(id);
        RemoveIfEmpty(zoneName);

        Save();
        return true;
    }

    private void RemoveIfEmpty(string zoneName)
    {
        if (StorageItem.All(x => x.ZoneName != zoneName))
        {
            return;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zoneName);

        if (zone.Nodes.Count == 0)
        {
            return;
        }
        
        RemoveZone(zoneName);
    }
    
    public List<Zone> GetZones() =>
        StorageItem;

    public Zone GetZone(string zoneName) =>
        GetZones().Find(x => x.ZoneName == zoneName);

    public bool Exists(string zoneName) => 
        GetZones().Exists(x => x.ZoneName == zoneName);
}