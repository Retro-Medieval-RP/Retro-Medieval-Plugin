using System;

namespace LootChest.Models;

internal class SpawnedChest
{
    public float LocX { get; set; }
    public float LocY { get; set; }
    public float LocZ { get; set; }
    
    public DateTime SpawnedDateTime { get; set; }
}