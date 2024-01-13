using TheLostLand.Core.Storage;
using TheLostLand.Models.Zones;
using UnityEngine;

namespace TheLostLand.Modules.Zones;

public class ZonesStorage : IStorage
{
    private List<Zone> Zones { get; set; } = [];

    public void Save()
    {
    }

    public void Load()
    {
    }

    public void CreateZone(string zone)
    {
        Zones.Add(new Zone
        {
            ZoneName = zone
        });

        Save();
    }

    public void DeleteZone(string zone)
    {
        Zones.RemoveAll(x => x.ZoneName == zone);

        Save();
    }

    public void DeleteNode(string zone, int id)
    {
        if (Zones.All(x => x.ZoneName != zone))
        {
            return;
        }

        var z = Zones.Find(x => x.ZoneName == zone);
        z.Nodes.RemoveAt(id);

        Save();
    }

    public void AddNode(string zone, Vector3 position)
    {
        if (Zones.All(x => x.ZoneName != zone))
        {
            return;
        }

        var z = Zones.Find(x => x.ZoneName == zone);
        z.Nodes.Add(new Node
        {
            X = position.x,
            Z = position.z
        });

        Save();
    }

    public IEnumerable<Zone> GetZones() => Zones;
}