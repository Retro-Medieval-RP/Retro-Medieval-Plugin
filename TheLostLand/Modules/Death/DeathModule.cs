using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
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

    protected override void OnTimerTick()
    {
        if (!GetStorage<DeathsStorage>(out var storage))
        {
            return;
        }
        
        if (!GetConfiguration<DeathsConfiguration>(out var config))
        {
            Logger.LogError("Could not gather configuration [DeathsConfiguration]");
            return;
        }

        var to_keep = storage.StorageItem.Where(body => !((DateTime.Now - body.BodySpawnTime).TotalMilliseconds >= config.DespawnTime)).ToList();
        storage.Set(to_keep);
    }

    private void OnGesture(GestureEventEventArgs e, ref bool allow)
    {
        if (e.Gesture != EPlayerGesture.POINT)
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
        if (e.Amount >= e.Player.life.health)
        {
            SendDeath(UnturnedPlayer.FromPlayer(e.Player));
        }
    }

    private void SaveInventory(Transform model, List<DeathItem> player_items)
    {
        var inv = new Body
        {
            BodySpawnTime = DateTime.Now,
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

        var man = barricade_drop.interactable as InteractableMannequin;
        if (man == null|| player_items.Count <= 0)
        {
            BarricadeManager.tryGetRegion(barricade_drop.model, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricade_drop, x, y, plant);
            return;
        }

        man.updateClothes(
            player.Player.clothing.shirt, player.Player.clothing.shirtQuality, player.Player.clothing.shirtState,
            player.Player.clothing.pants, player.Player.clothing.pantsQuality, player.Player.clothing.pantsState,
            player.Player.clothing.hat, player.Player.clothing.hatQuality, player.Player.clothing.hatState,
            player.Player.clothing.backpack, player.Player.clothing.backpackQuality,
            player.Player.clothing.backpackState,
            player.Player.clothing.vest, player.Player.clothing.vestQuality, player.Player.clothing.vestState,
            player.Player.clothing.mask, player.Player.clothing.maskQuality, player.Player.clothing.maskState,
            player.Player.clothing.glasses, player.Player.clothing.glassesQuality, player.Player.clothing.glassesState
        );

        man.rebuildState();

        var block = new Block();
        block.write(man.owner, man.group);
        block.writeInt32(man.visualShirt);
        block.writeInt32(man.visualPants);
        block.writeInt32(man.visualHat);
        block.writeInt32(man.visualBackpack);
        block.writeInt32(man.visualVest);
        block.writeInt32(man.visualMask);
        block.writeInt32(man.visualGlasses);
        block.writeUInt16(man.clothes.shirt);
        block.writeByte(man.shirtQuality);
        block.writeUInt16(man.clothes.pants);
        block.writeByte(man.pantsQuality);
        block.writeUInt16(man.clothes.hat);
        block.writeByte(man.hatQuality);
        block.writeUInt16(man.clothes.backpack);
        block.writeByte(man.backpackQuality);
        block.writeUInt16(man.clothes.vest);
        block.writeByte(man.vestQuality);
        block.writeUInt16(man.clothes.mask);
        block.writeByte(man.maskQuality);
        block.writeUInt16(man.clothes.glasses);
        block.writeByte(man.glassesQuality);
        block.writeByteArray(man.shirtState);
        block.writeByteArray(man.pantsState);
        block.writeByteArray(man.hatState);
        block.writeByteArray(man.backpackState);
        block.writeByteArray(man.vestState);
        block.writeByteArray(man.maskState);
        block.writeByteArray(man.glassesState);
        block.writeByte(man.pose_comp);
        BarricadeManager.updateReplicatedState(man.transform, block.getBytes(out var size), size);

        SaveInventory(barricade_drop.model, player_items);
        Main.Instance.StartCoroutine(ClearInventoryCoroutine(player.SteamPlayer()));
    }

    private static IEnumerator ClearInventoryCoroutine(SteamPlayer player)
    {
        for (byte page = 0; page < 6; page++)
        {
            for (byte i = 0; i < player.player.inventory.items[page].getItemCount(); i++)
            {
                var item = player.player.inventory.items[page].getItem(i);
                player.player.inventory.removeItem(page, player.player.inventory.getIndex(page, item.x, item.y));
            }
        }

        var remove_unequipped = () =>
        {
            for (byte i = 0; i < player.player.inventory.getItemCount(2); i++)
            {
                player.player.inventory.removeItem(2, 0);
            }
        };

        player.player.clothing.askWearBackpack(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearGlasses(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearHat(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearPants(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearMask(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearShirt(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        player.player.clothing.askWearVest(0, 0, Array.Empty<byte>(), true);
        remove_unequipped();
        yield return null;
    }
}