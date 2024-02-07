using System.Collections.Generic;

namespace RetroMedieval.Models.LootChest;

internal class Location
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }
    public float RotW { get; set; }
    
    public List<LootChestFlags> Flags { get; set; }
}