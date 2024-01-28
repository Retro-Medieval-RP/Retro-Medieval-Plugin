using System.Collections.Generic;
using System.Linq;
using SDG.Unturned;
using TheLostLand.Events.Zones;
using TheLostLand.Models.LootChest;
using TheLostLand.Models.Zones;
using TheLostLand.Modules.Attributes;
using TheLostLand.Utils;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.LootChest;

[ModuleInformation("LootChest")]
[ModuleConfiguration<LootChestConfiguration>("LootChestConfig")]
[ModuleStorage<LootChestLocationStorage>("LocationsStorage")]
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
    }
    
    public override void Unload()
    {
        ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEntered;
        ZoneLeftEventPublisher.ZoneLeftEvent -= OnZoneLeft;
    }

    private void OnZoneLeft(ZoneLeftEventArgs e)
    {
        if (!_lootChest.TryGetValue(e.Zone, out var chests))
        {
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
        }
    }
    
    private void OnZoneEntered(ZoneEnterEventArgs e)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage [LootChestLocationStorage]");
            return;
        }

        if (storage.StorageItem.All(x => x.ZoneName != e.Zone.ZoneName))
        {
            return;
        }

        var chest_locations = storage.StorageItem.Find(x => x.ZoneName == e.Zone.ZoneName);
        foreach (var chest in chest_locations.Locations)
        {
            SpawnChest(chest, out var trans);

            if (_lootChest.ContainsKey(e.Zone))
            {
                _lootChest[e.Zone].Add(trans);
                continue;
            }
            
            _lootChest.Add(e.Zone, [trans]);
        }
    }

    private void SpawnChest(Location chest_location, out Transform transform)
    {
        var chest = _chestPicker.GetRandom();
        
        var chest_point = new Vector3(chest_location.X, chest_location.Y, chest_location.Z);
        var chest_angle = Quaternion.Euler(90, chest_location.Rot, 0);
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

    public bool AddChest(string zone_name, Vector3 position, float rotation, out int id)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            id = default;
            return false;
        }

        storage.AddLocation(zone_name, position, rotation);
        var location = storage.GetLocations(zone_name);
        id = location.Locations.Count - 1;

        return true;
    }

    public bool RemoveChest(string zone_name, int id)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            return false;
        }

        storage.RemoveLocation(zone_name, id);
        return true;
    }
}