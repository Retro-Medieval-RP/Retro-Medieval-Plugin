using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LootChest.Events;
using LootChest.Models;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Zones;
using RetroMedieval.Shared.Models.Zones;
using RetroMedieval.Utils;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace LootChest;

[ModuleInformation("LootChest")]
[ModuleConfiguration<LootChestConfiguration>("LootChestConfig")]
[ModuleStorage<LootChestLocationStorage>("LocationsStorage")]
[ModuleStorage<LootChestSpawnedStorage>("SpawnedLootChests")]
internal class LootChestModule([NotNull] string directory) : Module(directory)
{
    private readonly Picker<Chest> _chestPicker = new();
    private readonly Dictionary<Zone, List<Transform>> _lootChest = [];
    
    public override void Load()
    {
        if (!GetConfiguration<LootChestConfiguration>(out var config))
        {
            Logger.LogError("Could not gather configuration [LootChestConfiguration]");
            return;
        }

        foreach (var chest in config.Chests)
        {
            _chestPicker.AddEntry(chest, chest.ChestSpawnChance);
        }
        
        ZoneEnterEventPublisher.ZoneEnterEvent += OnZoneEntered;
        ZoneLeftEventPublisher.ZoneLeftEvent += OnZoneLeft;
        SpawnLootChestEventPublisher.SpawnLootChestEvent += ChestTo;
        RemoveLootChestEventPublisher.RemoveLootChestEvent += ChestTo;
    }

    public override void Unload()
    {
        ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEntered;
        ZoneLeftEventPublisher.ZoneLeftEvent -= OnZoneLeft;
        SpawnLootChestEventPublisher.SpawnLootChestEvent -= ChestTo;
        RemoveLootChestEventPublisher.RemoveLootChestEvent -= ChestTo;
    }

    protected override void OnTimerTick()
    {
        if (!GetStorage<LootChestSpawnedStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage [LootChestSpawnedStorage]");
            return;
        }

        if (!GetConfiguration<LootChestConfiguration>(out var config))
        {
            Logger.LogError("Could not gather configuration [LootChestConfiguration]");
            return;
        }

        var expired = storage.GetExpiredChests(config.DespawnRate);
        storage.NewLoadOfChests(storage.StorageItem.Where(x => !expired.Contains(x)));

        foreach (var exp in expired)
        {
            var chestPos = new Vector3(exp.LocX, exp.LocY, exp.LocZ);
            var trans = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(chestPos, 1, trans);

            foreach (var tran in trans)
            {
                if (tran.position != chestPos)
                {
                    continue;
                }
                
                var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(tran);
                BarricadeManager.tryGetRegion(tran, out var x, out var y, out var plant, out _);
                BarricadeManager.destroyBarricade(barricadeDrop, x, y, plant);
            }
        }
    }

    private void ChestTo(SpawnLootChestEventArgs e)
    {
        if (!e.Zone(out var zone))
        {
            return;
        }
        
        SpawnNewLootChests(zone, e.Flag);
    }
    
    private void ChestTo(RemoveLootChestEventArgs e)
    {
        if (!e.Zone(out var zone))
        {
            return;
        }
        
        RemoveLootChestsInZone(zone);
    }
    
    private void OnZoneLeft(ZoneLeftEventArgs e) => 
        RemoveLootChestsInZone(e.Zone);

    private void OnZoneEntered(ZoneEnterEventArgs e) => 
        SpawnNewLootChests(e.Zone, LootChestFlags.ZoneEntered);
    
    private void RemoveLootChestsInZone(Zone e)
    {
        if (!_lootChest.TryGetValue(e, out var chests))
        {
            return;
        }

        if (!GetStorage<LootChestSpawnedStorage>(out var spawnedStorage))
        {
            Logger.LogError("Could not gather storage [LootChestSpawnedStorage]");
            return;
        }
        
        foreach (var chest in chests)
        {
            var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(chest);
            
            var storage = barricadeDrop.interactable as InteractableStorage;
            if (storage != null)
            {
                while (storage.items.items.Count > 0)
                {
                    var itemJar = storage.items.items.First();
                    storage.items.items.RemoveAt(storage.items.getIndex(itemJar.x, itemJar.y));
                }
            }

            BarricadeManager.tryGetRegion(chest, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricadeDrop, x, y, plant);

            var position = chest.position;
            spawnedStorage.RemoveChest(position.x, position.y, position.z);
        }
    }
    
    private void SpawnNewLootChests(Zone e, LootChestFlags flag)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage [LootChestLocationStorage]");
            return;
        }
        
        if (!GetStorage<LootChestSpawnedStorage>(out var spawnedStorage))
        {
            Logger.LogError("Could not gather storage [LootChestSpawnedStorage]");
            return;
        }

        if (storage.StorageItem.All(x => x.ZoneName != e.ZoneName))
        {
            return;
        }

        if (!storage.StorageItem.Find(x => x.ZoneName == e.ZoneName).Flags.Contains(flag))
        {
            return;
        }
        
        var chestLocations = storage.StorageItem.Find(x => x.ZoneName == e.ZoneName);
        foreach (var chest in chestLocations.Locations)
        {
            SpawnChest(chest, out var trans);
            spawnedStorage.AddedChest(new SpawnedChest
            {
                LocX = trans.position.x,
                LocY = trans.position.y,
                LocZ = trans.position.z,
                SpawnedDateTime = DateTime.Now
            });

            if (_lootChest.ContainsKey(e))
            {
                _lootChest[e].Add(trans);
                continue;
            }
            
            _lootChest.Add(e, [trans]);
        }
    }

    private void SpawnChest(Location chestLocation, out Transform transform)
    {
        var chest = _chestPicker.GetRandom();
        
        var chestPoint = new Vector3(chestLocation.X, chestLocation.Y, chestLocation.Z);
        var chestAngle = new Quaternion(chestLocation.RotX, chestLocation.RotY, chestLocation.RotZ, chestLocation.RotW);
        var barricade = new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, chest.ChestBarricade));
        transform = BarricadeManager.dropNonPlantedBarricade(barricade, chestPoint, chestAngle, 0, 0);
        
        var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(transform);
        
        if (barricadeDrop.interactable as InteractableStorage == null)
        {
            BarricadeManager.tryGetRegion(transform, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricadeDrop, x, y, plant);
            return;
        }

        InsertItems(chest, AddItemsToPicker(chest), barricadeDrop.interactable as InteractableStorage);
    }

    private static Picker<LootItem> AddItemsToPicker(Chest chest)
    {
        var itemPicker = new Picker<LootItem>();

        foreach (var lootItem in chest.SpawnTable)
        {
            itemPicker.AddEntry(lootItem, lootItem.SpawnChance);
        }

        return itemPicker;
    }

    private static void InsertItems(Chest chest, Picker<LootItem> itemPicker, InteractableStorage storage)
    {
        var itemsToSpawn = new List<LootItem>();
        for (var i = 0; i < chest.AmountToPick; i++)
        {
            itemsToSpawn.Add(itemPicker.GetRandom());
        }

        foreach (var item in itemsToSpawn)
        {
            storage!.items.tryAddItem(new Item(item.LootItemID, true));
        }
    }

    public bool AddChest(string zoneName, Vector3 position, Quaternion rotation, string flags, out int id)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            id = default;
            return false;
        }
        
        var flagsList = new List<LootChestFlags>();
        foreach(var flag in flags.Split('¬'))
        {
            if (Enum.TryParse<LootChestFlags>(flag, out var flagEnum))
            {
                flagsList.Add(flagEnum);
            }
        }
        
        storage.AddLocation(zoneName, position, rotation, flagsList);
        var location = storage.GetLocations(zoneName);
        id = location.Locations.Count - 1;

        return true;
    }

    public bool RemoveChest(string zoneName, int id) => 
        GetStorage<LootChestLocationStorage>(out var storage) && storage.RemoveLocation(zoneName, id);
}