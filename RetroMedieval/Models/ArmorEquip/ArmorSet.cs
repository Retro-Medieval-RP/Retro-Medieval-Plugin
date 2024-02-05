using System.Collections.Generic;

namespace RetroMedieval.Models.ArmorEquip;

internal class ArmorSet
{
    public bool ReplaceClothing { get; set; }
    public bool DropInventoryWhenEquip { get; set; }
    public ushort MainItem { get; set; }
    public List<ushort> Items { get; set; }
}