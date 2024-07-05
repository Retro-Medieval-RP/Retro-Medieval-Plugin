using UnityEngine;

namespace Zones.Models;

public class Node
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Node()
    {
    }
    
    public Node(Vector3 position) : this(position.x, position.y, position.z)
    {
    }

    private Node(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}