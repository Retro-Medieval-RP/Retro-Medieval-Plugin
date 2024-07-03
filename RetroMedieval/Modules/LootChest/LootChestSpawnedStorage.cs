using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RetroMedieval.Models.LootChest;
using RetroMedieval.Savers.Json;

namespace RetroMedieval.Modules.LootChest;

internal class LootChestSpawnedStorage : JsonSaver<List<SpawnedChest>>
{
    public bool AddedChest(SpawnedChest chest)
    {
        try
        {
            StorageItem.Add(chest);
            Save();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool NewLoadOfChests(IEnumerable<SpawnedChest> chests)
    {
        try
        {
            StorageItem = chests.ToList();
            Save();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<SpawnedChest> GetExpiredChests(int despawnTime) =>
        StorageItem.Where(chest => (DateTime.Now - chest.SpawnedDateTime).Milliseconds >= despawnTime);

    public void RemoveChest(float positionX, float positionY, float positionZ)
    {
        var posVector = new Vector3(positionX, positionY, positionZ);

        if (StorageItem.Any(x => new Vector3(x.LocX, x.LocY, x.LocZ).Equals(posVector)))
        {
            StorageItem.RemoveAll(x => new Vector3(x.LocX, x.LocY, x.LocZ).Equals(posVector));
        }
    }
}