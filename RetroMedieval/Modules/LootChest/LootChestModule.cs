using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Events.LootChests;
using RetroMedieval.Events.Zones;
using RetroMedieval.Models.LootChest;
using RetroMedieval.Models.Zones;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Utils;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Modules.LootChest;

[ModuleInformation("LootChest")]
[ModuleConfiguration<LootChestConfiguration>("LootChestConfig")]
[ModuleStorage<LootChestLocationStorage>("LocationsStorage")]
[ModuleStorage<LootChestSpawnedStorage>("SpawnedLootChests")]
internal class LootChestModule : Module
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
            var chest_pos = new Vector3(exp.LocX, exp.LocY, exp.LocZ);
            var trans = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(chest_pos, 1, trans);

            foreach (var tran in trans)
            {
                if (tran.position != chest_pos)
                {
                    continue;
                }
                
                var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(tran);
                BarricadeManager.tryGetRegion(tran, out var x, out var y, out var plant, out _);
                BarricadeManager.destroyBarricade(barricade_drop, x, y, plant);
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

        if (!GetStorage<LootChestSpawnedStorage>(out var spawned_storage))
        {
            Logger.LogError("Could not gather storage [LootChestSpawnedStorage]");
            return;
        }
        
        foreach (var chest in chests)
        {
            var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(chest);
            
            var storage = barricade_drop.interactable as InteractableStorage;
            if (storage != null)
            {
                while (storage.items.items.Count > 0)
                {
                    var item_jar = storage.items.items.First();
                    storage.items.items.RemoveAt(storage.items.getIndex(item_jar.x, item_jar.y));
                }
            }

            BarricadeManager.tryGetRegion(chest, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricade_drop, x, y, plant);

            var position = chest.position;
            spawned_storage.RemoveChest(position.x, position.y, position.z);
        }
    }
    
    private void SpawnNewLootChests(Zone e, LootChestFlags flag)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage [LootChestLocationStorage]");
            return;
        }
        
        if (!GetStorage<LootChestSpawnedStorage>(out var spawned_storage))
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
        
        var chest_locations = storage.StorageItem.Find(x => x.ZoneName == e.ZoneName);
        foreach (var chest in chest_locations.Locations)
        {
            SpawnChest(chest, out var trans);
            spawned_storage.AddedChest(new SpawnedChest
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

    private void SpawnChest(Location chest_location, out Transform transform)
    {
        var chest = _chestPicker.GetRandom();
        
        var chest_point = new Vector3(chest_location.X, chest_location.Y, chest_location.Z);
        var chest_angle = new Quaternion(chest_location.RotX, chest_location.RotY, chest_location.RotZ, chest_location.RotW);
        var barricade = new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, chest.ChestBarricade));
        transform = BarricadeManager.dropNonPlantedBarricade(barricade, chest_point, chest_angle, 0, 0);
        
        var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(transform);
        
        if (barricade_drop.interactable as InteractableStorage == null)
        {
            BarricadeManager.tryGetRegion(transform, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricade_drop, x, y, plant);
            return;
        }

        InsertItems(chest, AddItemsToPicker(chest), barricade_drop.interactable as InteractableStorage);
    }

    private static Picker<LootItem> AddItemsToPicker(Chest chest)
    {
        var item_picker = new Picker<LootItem>();

        foreach (var loot_item in chest.SpawnTable)
        {
            item_picker.AddEntry(loot_item, loot_item.SpawnChance);
        }

        return item_picker;
    }

    private static void InsertItems(Chest chest, Picker<LootItem> item_picker, InteractableStorage storage)
    {
        var items_to_spawn = new List<LootItem>();
        for (var i = 0; i < chest.AmountToPick; i++)
        {
            items_to_spawn.Add(item_picker.GetRandom());
        }

        foreach (var item in items_to_spawn)
        {
            storage!.items.tryAddItem(new Item(item.LootItemID, true));
        }
    }

    public bool AddChest(string zone_name, Vector3 position, Quaternion rotation, string flags, out int id)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            id = default;
            return false;
        }
        
        var flags_list = new List<LootChestFlags>();
        foreach(var flag in flags.Split('¬'))
        {
            if (Enum.TryParse<LootChestFlags>(flag, out var flag_enum))
            {
                flags_list.Add(flag_enum);
            }
        }
        
        storage.AddLocation(zone_name, position, rotation, flags_list);
        var location = storage.GetLocations(zone_name);
        id = location.Locations.Count - 1;

        return true;
    }

    public bool RemoveChest(string zone_name, int id) => 
        GetStorage<LootChestLocationStorage>(out var storage) && storage.RemoveLocation(zone_name, id);
}