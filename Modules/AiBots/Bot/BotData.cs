using Newtonsoft.Json;
using UnityEngine;

namespace AiBots.Bot;

internal class BotData
{
    public ulong Id { get; set; }

    [JsonIgnore]
    public Vector3 Position
    {
        get => new(X, Y, Z);
        set
        {
            X = value.x;
            Y = value.y;
            Z = value.z;
        }
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}