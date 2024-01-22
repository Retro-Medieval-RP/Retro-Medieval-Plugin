﻿using Rocket.Core.Logging;
using TheLostLand.Core.Modules;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Core.Utils;
using TheLostLand.Events.Zones;
using TheLostLand.Models.LootChest;

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

    private static void OnZoneEntered(ZoneEnterEventArgs e)
    {
    }

    public override void Unload()
    {
        ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEntered;
    }
}