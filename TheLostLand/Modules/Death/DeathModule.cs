using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Models.Death;
using TheLostLand.Modules.Attributes;
using TheLostLand.Utils;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.Death;

[ModuleInformation("Deaths")]
[ModuleConfiguration<DeathsConfiguration>("DeathsConfiguration")]
[ModuleStorage<DeathsStorage>("DeathsStorage")]
public class DeathModule : Module
{
    public override void Load()
    {
        UnturnedPlayerEvents.OnPlayerDead += OnPlayerDeath;
        UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerGesture;
    }

    private void OnPlayerDeath(UnturnedPlayer player, Vector3 position)
    {
        var player_items = new List<DeathItem>();

        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                var item = player.Inventory.getItem(i, 0);
                player_items.Add(new DeathItem(item.item.id, item.item.amount, item.item.quality, item.item.state));
                player.Inventory.removeItem(i, 0);
            }
        }

        var barricade_drop = PlaceBarricade(player);

        if (barricade_drop.interactable as InteractableMannequin == null)
        {
            BarricadeManager.tryGetRegion(barricade_drop.model, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricade_drop, x, y, plant);
            return;
        }

        var man = barricade_drop.interactable as InteractableMannequin;

        if (man == null)
        {
            return;
        }
        
        man.clothes.backpack = player.Player.clothing.backpack;
        man.clothes.hat = player.Player.clothing.hat;
        man.clothes.shirt = player.Player.clothing.shirt;
        man.clothes.pants = player.Player.clothing.pants;
        man.clothes.vest = player.Player.clothing.vest;
        man.clothes.glasses = player.Player.clothing.glasses;
        man.clothes.mask = player.Player.clothing.mask;

        SaveInventory(barricade_drop.model, player_items);
    }

    public override void Unload()
    {
        UnturnedPlayerEvents.OnPlayerDead -= OnPlayerDeath;
        UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerGesture;
    }
    
    private void OnPlayerGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
    {
        if (gesture != UnturnedPlayerEvents.PlayerGesture.PunchLeft && gesture != UnturnedPlayerEvents.PlayerGesture.PunchRight)
        {
            return;
        }
        
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return;
        }

        var drop = BarricadeManager.FindBarricadeByRootTransform(result.BarricadeRootTransform);
        
        if (!GetStorage<DeathsStorage>(out var storage))
        {
            return;
        }

        if (!storage.InventoryAt(drop.model.position))
        {
            return;
        }

        var inv = storage.GetInv(drop.model.position);

        var inventory = new Items(7);
        inventory.resize(8, 100);

        foreach (var item in inv.Items)
        {
            inventory.tryAddItem(new Item(item.Item, item.Amount, item.Quality, item.State));
        }
        
        player.Inventory.updateItems(7, inventory);
        player.Inventory.sendStorage();
    }

    private void SaveInventory(Transform model, List<DeathItem> player_items)
    {
        var inv = new Inventory
        {
            Items = player_items,
            LocX = model.position.x,
            LocY = model.position.y,
            LocZ = model.position.z
        };
        
        if (!GetStorage<DeathsStorage>(out var storage))
        {
            return;
        }

        storage.AddInventory(inv);
    }

    private BarricadeDrop PlaceBarricade(UnturnedPlayer player)
    {
        if (!GetConfiguration<DeathsConfiguration>(out var config))
        {
            Logger.LogError("Could not gather configuration [DeathsConfiguration]");
            return null;
        }
        
        var barri_angle = new Quaternion(0f, 0f, 0f, 0f);
        var barricade = new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, config.ManID));
        var transform = BarricadeManager.dropNonPlantedBarricade(barricade, player.Position, barri_angle, 0, 0);
        
        var barricade_drop = BarricadeManager.FindBarricadeByRootTransform(transform);
        return barricade_drop;
    }
}