using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.DeadBodys;
using RetroMedieval.Savers;
using RetroMedieval.Savers.Json;
using UnityEngine;

namespace RetroMedieval.Modules.DeadBodys;

public class DeathsStorage : JsonSaver<List<Body>>
{
    public void AddInventory(Body inv)
    {
        StorageItem.Add(inv);
        Save();
    }

    public bool InventoryAt(Vector3 position) => 
        StorageItem.Any(x => new Vector3(x.LocX, x.LocY, x.LocZ) == position);

    public Body GetInv(Vector3 position) => 
        StorageItem.Find(x => new Vector3(x.LocX, x.LocY, x.LocZ) == position);
}