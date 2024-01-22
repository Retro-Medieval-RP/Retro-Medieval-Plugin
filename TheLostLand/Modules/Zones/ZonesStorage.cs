using System.Collections.Generic;
using System.Linq;
using TheLostLand.Core.Savers;
using TheLostLand.Models.Zones;

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

    public void RemoveNode(string zone_name, int id)
    {
        if (StorageItem.All(x => x.ZoneName != zone_name))
        {
            return;
        }

        var zone = StorageItem.Find(x => x.ZoneName == zone_name);
        zone.Nodes.RemoveAt(id);

        Save();
    }

    public List<Zone> GetZones() =>
        StorageItem;

    public Zone GetZone(string zone_name) =>
        GetZones().Find(x => x.ZoneName == zone_name);
}