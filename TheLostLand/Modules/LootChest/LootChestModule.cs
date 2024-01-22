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

    private void SpawnChest(Location chest)
    {
    }
}