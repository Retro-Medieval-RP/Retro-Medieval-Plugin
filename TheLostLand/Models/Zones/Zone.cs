using System.Collections.Generic;
using UnityEngine;

namespace TheLostLand.Models.Zones;

public class Zone
{
    public string ZoneName { get; set; }
    public List<Node> Nodes { get; set; }

    public Zone(string name, List<Node> nodes)
    {
        ZoneName = name;
        Nodes = nodes;
    }

    public Zone()
    {
    }

    public bool IsInZone(Vector3 point)
    {
        var result = false;
        var j = Nodes.Count - 1;
        for (var i = 0; i < Nodes.Count; i++)
        {
            if ((Nodes[i].Z < point.z && Nodes[j].Z >= point.z || Nodes[j].Z < point.z && Nodes[i].Z >= point.z) && Nodes[i].X + (point.z - Nodes[i].Z) / (Nodes[j].Z - Nodes[i].Z) * (Nodes[j].X - Nodes[i].X) < point.x)
            {
                result = !result;
            }
            j = i;
        }
        return result;
    }
}