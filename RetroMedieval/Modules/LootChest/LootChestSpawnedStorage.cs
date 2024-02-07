using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RetroMedieval.Models.LootChest;
using RetroMedieval.Savers;

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

    public IEnumerable<SpawnedChest> GetExpiredChests(int despawn_time) =>
        StorageItem.Where(chest => (DateTime.Now - chest.SpawnedDateTime).Milliseconds >= despawn_time);

    public void RemoveChest(float position_x, float position_y, float position_z)
    {
        var pos_vector = new Vector3(position_x, position_y, position_z);

        if (StorageItem.Any(x => new Vector3(x.LocX, x.LocY, x.LocZ).Equals(pos_vector)))
        {
            StorageItem.RemoveAll(x => new Vector3(x.LocX, x.LocY, x.LocZ).Equals(pos_vector));
        }
    }
}