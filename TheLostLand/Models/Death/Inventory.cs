using System.Collections.Generic;

namespace TheLostLand.Models.Death;

public class Inventory
{
    public float LocX { get; set; }
    public float LocY { get; set; }
    public float LocZ { get; set; }
    
    public List<DeathItem> Items { get; set; }
}