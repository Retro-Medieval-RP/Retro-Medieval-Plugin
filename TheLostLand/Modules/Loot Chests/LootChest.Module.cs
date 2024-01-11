using SDG.Unturned;
using UnityEngine;

namespace TheLostLand.Modules.Loot_Chests;

[ModuleInformation("Loot Module")]
[ModuleStorage<LootChestLocationStorage>("LootChestLocation")]
[ModuleConfiguration<LootChestModuleConfig>("LootChestConfiguration")]
public class LootChestModule : Module
{
    private Picker<LootChest> Picker { get; set; } = new();

    public LootChestModule()
    {
        if (!GetConfiguration<LootChestModuleConfig>(out var config)) return;

        foreach (var chest in config.LootChests)
        {
            Picker.AddEntry(chest, chest.ChestPlaceChance);
        }
    }

    public void AddChestLocation(Vector3 location, string zone)
    {
        if (GetStorage<LootChestLocationStorage>(out var storage))
        {
            storage.AddLocation(location, zone);
        }
    }

    public void RemoveChestLocation(Vector3 position)
    {
        if (GetStorage<LootChestLocationStorage>(out var storage))
        {
            storage.RemoveLocation(position);
        }
    }

    public void SpawnChests(string zone)
    {
        if (!GetStorage<LootChestLocationStorage>(out var storage))
        {
            return;
        }
        
        var locations = storage.GetLocationsFromZone(zone);

        foreach (var location in locations)
        {
            GenerateChest(location);
        }
    }

    private void GenerateChest(LootChestLocation location)
    {
        var chest = Picker.GetRandom();

        var barricade_transform = BarricadeManager.dropNonPlantedBarricade(new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, chest.ChestID)),
            new Vector3(location.X, location.Y, location.Z), new Quaternion(0, 0, 0, 0), 0, 0);

        var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(barricade_transform);

        var storage = barricade_drop.interactable as InteractableStorage;

        if (storage == null)
        {
            return;
        }

        var item_picker = new Picker<LootItem>();
        foreach (var item in chest.LootItems)
        {
            item_picker.AddEntry(item, item.SpawnChance);
        }

        var items = GetItems(item_picker, 5);
        foreach (var item in items)
        {
            storage.items.tryAddItem(new Item(item.ItemID, true));
        }
    }

    private static IEnumerable<LootItem> GetItems(Picker<LootItem> item_picker, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return item_picker.GetRandom();
        }
    }
}