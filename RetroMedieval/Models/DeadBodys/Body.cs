using System;
using System.Collections.Generic;

namespace RetroMedieval.Models.DeadBodys;

public class Body
{
    public DateTime BodySpawnTime { get; set; }
    
    public float LocX { get; set; }
    public float LocY { get; set; }
    public float LocZ { get; set; }
    
    public List<Item> Items { get; set; }
}