using System.Collections.Generic;

namespace ArmorEquip.Models;

internal class ArmorSet
{
    public bool DropInventoryWhenEquip { get; set; }
    public ushort MainItem { get; set; }
    public List<ushort> Items { get; set; }
}