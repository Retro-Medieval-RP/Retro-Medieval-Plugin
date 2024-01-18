using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TheLostLand.Core.Modules.Storage;
using TheLostLand.Models.Zones;
using UnityEngine;

namespace TheLostLand.Modules.Zones;

public class ZonesStorage : IStorage
{
    private List<Zone> Zones { get; set; }
    private string FilePath { get; set; }

    public bool Load(string file_path)
    {
        FilePath = file_path;

        if (File.Exists(FilePath))
        {
            string data_text;
            using (var stream = File.OpenText(FilePath))
            {
                data_text = stream.ReadToEnd();
            }

            Zones = JsonConvert.DeserializeObject<List<Zone>>(data_text);
            return true;
        }

        Zones = [];
        Save();

        return true;
    }

    public void Save()
    {
        var obj_data = JsonConvert.SerializeObject(Zones, Formatting.Indented);

        using var stream = new StreamWriter(FilePath, false);
        stream.Write(obj_data);
    }

    public void AddZone(string zone_name)
    {
        Zones.Add(new Zone(zone_name, []));
        Save();
    }

    public void RemoveZone(string zone_name)
    {
        Zones.RemoveAll(x => x.ZoneName == zone_name);
        Save();
    }

    public void AddNode(string zone_name, Node node)
    {
        if (Zones.All(x => x.ZoneName != zone_name))
        {
            return;
        }

        var zone = Zones.Find(x => x.ZoneName == zone_name);
        zone.Nodes.Add(node);
        
        Save();
    }

    public void RemoveNode(string zone_name, int id)
    {
        if (Zones.All(x => x.ZoneName != zone_name))
        {
            return;
        }

        var zone = Zones.Find(x => x.ZoneName == zone_name);
        zone.Nodes.RemoveAt(id);
        
        Save();
    }

    public List<Zone> GetZones() => 
        Zones;

    public Zone GetZone(string zone_name) => 
        GetZones().Find(x => x.ZoneName == zone_name);
}