using UnityEngine;

namespace TheLostLand.Models.Zones;

public class Zone
{
    internal string ZoneName { get; set; }
    internal List<Node> Nodes { get; set; } = [];

    internal bool IsPointInZone(Vector3 point_to_check)
    {
        var result = false;
        var j = Nodes.Count - 1;
        for (var i = 0; i < Nodes.Count; i++)
        {
            if ((Nodes[i].Z < point_to_check.z && Nodes[j].Z >= point_to_check.z ||
                 Nodes[j].Z < point_to_check.z && Nodes[i].Z >= point_to_check.z) &&
                Nodes[i].X + (point_to_check.z - Nodes[i].Z) / (Nodes[j].Z - Nodes[i].Z) * (Nodes[j].X - Nodes[i].X) <
                point_to_check.x)
                result = !result;
            j = i;
        }

        return result;
    }
}