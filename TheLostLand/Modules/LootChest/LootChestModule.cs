using System.Collections.Generic;
using System.Linq;
using SDG.Unturned;
using TheLostLand.Core.Modules;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Core.Utils;
using TheLostLand.Events.Zones;
using TheLostLand.Models.LootChest;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.LootChest;

[ModuleInformation("LootChest")]
[ModuleConfiguration<LootChestConfiguration>("LootChestConfig")]
[ModuleStorage<LootChestLocationStorage>("LocationsStorage")]
internal class LootChestModule : Module
{
    private readonly Picker<Chest> _chestPicker = new();
    
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
    }
    
    public override void Unload()
    {
        ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEntered;
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
            SpawnChest(chest);
        }
    }

    private void SpawnChest(Location chest_location)
    {
        var chest = _chestPicker.GetRandom();
        
        var chest_point = new Vector3(chest_location.X, chest_location.Y, chest_location.Z);
        var chest_angle = new Quaternion(chest_location.AngleX, chest_location.AngleY, chest_location.AngleZ, chest_location.AngleW);
        var barricade = new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, chest.ChestBarricade));
        
        var barricade_transform = BarricadeManager.dropNonPlantedBarricade(barricade, chest_point, chest_angle, 0, 0);

        var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(barricade_transform);
        
        if (barricade_drop.interactable as InteractableStorage == null)
        {
            var data = BarricadeManager.tryGetRegion(barricade_transform, out var x, out var y, out var plant, out _);
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
}