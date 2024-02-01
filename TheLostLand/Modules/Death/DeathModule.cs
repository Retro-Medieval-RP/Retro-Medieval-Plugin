using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Events.Unturned;
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
        DamageEventEventPublisher.DamageEventEvent += OnDamage;
        GestureEventEventPublisher.GestureEventEvent += OnGesture;
    }
    
    public override void Unload()
    {
        DamageEventEventPublisher.DamageEventEvent -= OnDamage;
        GestureEventEventPublisher.GestureEventEvent -= OnGesture;
    }
    
    private void OnGesture(GestureEventEventArgs e, ref bool allow)
    {
        if (e.Gesture != EPlayerGesture.PUNCH_LEFT && e.Gesture != EPlayerGesture.PUNCH_RIGHT)
        {
            return;
        }
        
        var result = Raycaster.RayCastPlayer(e.Player, RayMasks.BARRICADE_INTERACT);
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
        
        e.Player.Inventory.updateItems(7, inventory);
        e.Player.Inventory.sendStorage();

    }
    
    private void OnDamage(DamageEventEventArgs e, ref EPlayerKill kill, ref bool allow)
    {
        if (e.Amount < e.Player.life.health)
        {
            return;
        }
        
        SendDeath(UnturnedPlayer.FromPlayer(e.Player));
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

    private void SendDeath(UnturnedPlayer player)
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
        
        man.updateClothes(
            player.Player.clothing.shirt, player.Player.clothing.shirtQuality, player.Player.clothing.shirtState,
            player.Player.clothing.pants, player.Player.clothing.pantsQuality, player.Player.clothing.pantsState,
            player.Player.clothing.hat, player.Player.clothing.hatQuality, player.Player.clothing.hatState,
            player.Player.clothing.backpack, player.Player.clothing.backpackQuality, player.Player.clothing.backpackState,
            player.Player.clothing.vest, player.Player.clothing.vestQuality, player.Player.clothing.vestState,
            player.Player.clothing.mask, player.Player.clothing.maskQuality, player.Player.clothing.maskState,
            player.Player.clothing.glasses, player.Player.clothing.glassesQuality, player.Player.clothing.glassesState
            );
        
        player.Player.clothing.askWearShirt(0, 0, [], false);
        player.Player.clothing.askWearPants(0, 0, [], false);
        player.Player.clothing.askWearHat(0, 0, [], false);
        player.Player.clothing.askWearBackpack(0, 0, [], false);
        player.Player.clothing.askWearVest(0, 0, [], false);
        player.Player.clothing.askWearMask(0, 0, [], false);
        player.Player.clothing.askWearGlasses(0, 0, [], false);
        
        man.clothes.apply();

        SaveInventory(barricade_drop.model, player_items);
    }
}