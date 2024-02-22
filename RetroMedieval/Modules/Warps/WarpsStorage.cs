using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Warps;
using RetroMedieval.Savers.Json;

namespace RetroMedieval.Modules.Warps;

internal class WarpsStorage : JsonSaver<List<Warp>>
{
    public void AddWarp(Warp warp)
    {
        StorageItem.Add(warp);
        Save();
    }

    public void RemoveWarp(string warp_name)
    {
        StorageItem.RemoveAll(x => string.Equals(x.WarpName, warp_name, StringComparison.CurrentCultureIgnoreCase));
        Save();
    }

    public bool ContainsWarp(string warp_name) => 
        StorageItem.Any(x => string.Equals(x.WarpName, warp_name, StringComparison.CurrentCultureIgnoreCase));

    public Warp GetWarp(string warp_name) => 
        StorageItem.Find(x => string.Equals(x.WarpName, warp_name, StringComparison.CurrentCultureIgnoreCase));
}