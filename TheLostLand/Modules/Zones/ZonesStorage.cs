using System.Collections.Generic;
using System.Linq;
using TheLostLand.Models.Zones;
using TheLostLand.Modules.Storage;
using TheLostLand.Savers;

namespace TheLostLand.Modules.Zones;

public class ZonesStorage : JsonSaver<List<Zone>>
{
    public void AddZone(string zone_name)
    {
        StorageItem.Add(new Zone(zone_name, []));
        Save();
    }

    public void RemoveZone(string zone_name)
    {
        StorageItem.RemoveAll(x => x.ZoneName == zone_name);
        Save();
    }

    public void AddNode(string zone_name, Node node)
    {
        if (StorageItem.All(x => x.ZoneName != zone_name))
        {
            return;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zone_name);
        zone.Nodes.Add(node);

        Save();
    }

    public bool RemoveNode(string zone_name, int id)
    {
        if (StorageItem.All(x => x.ZoneName != zone_name))
        {
            return false;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zone_name);

        if (zone.Nodes.Count == 0)
        {
            return false;
        }
        
        zone.Nodes.RemoveAt(id);
        RemoveIfEmpty(zone_name);

        Save();
        return true;
    }

    private void RemoveIfEmpty(string zone_name)
    {
        if (StorageItem.All(x => x.ZoneName != zone_name))
        {
            return;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zone_name);

        if (zone.Nodes.Count == 0)
        {
            return;
        }
        
        RemoveZone(zone_name);
    }
    
    public List<Zone> GetZones() =>
        StorageItem;

    public Zone GetZone(string zone_name) =>
        GetZones().Find(x => x.ZoneName == zone_name);

    public bool Exists(string zone_name) => 
        GetZones().Exists(x => x.ZoneName == zone_name);
}