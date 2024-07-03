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

    public void RemoveWarp(string warpName)
    {
        StorageItem.RemoveAll(x => string.Equals(x.WarpName, warpName, StringComparison.CurrentCultureIgnoreCase));
        Save();
    }

    public bool ContainsWarp(string warpName) => 
        StorageItem.Any(x => string.Equals(x.WarpName, warpName, StringComparison.CurrentCultureIgnoreCase));

    public Warp GetWarp(string warpName) => 
        StorageItem.Find(x => string.Equals(x.WarpName, warpName, StringComparison.CurrentCultureIgnoreCase));
}