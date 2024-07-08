using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeadBodies.Events;
using DeadBodies.Models;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned;
using RetroMedieval.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Item = DeadBodies.Models.Item;
using Logger = Rocket.Core.Logging.Logger;

namespace DeadBodies;

[ModuleInformation("Deaths")]
[ModuleConfiguration<DeathsConfiguration>("DeathsConfiguration")]
[ModuleStorage<DeathsStorage>("DeathsStorage")]
public class DeathModule([NotNull] string directory) : Module(directory)
{
    private Dictionary<(UnturnedPlayer, Vector3), Vector3> CurrentAccessedInvs { get; set; }
    
    public override void Load()
    {
        CurrentAccessedInvs = [];
        DamageEventEventPublisher.DamageEventEvent += OnDamage;
        GestureEventEventPublisher.GestureEventEvent += OnGesture;

        BarricadeManager.onDamageBarricadeRequested += OnDamageBarricade;
        
        SpawnDeadBodyEventPublisher.SpawnDeadBodyEvent += SpawnBody;
    }

    public override void Unload()
    {
        CurrentAccessedInvs.Clear();
        DamageEventEventPublisher.DamageEventEvent -= OnDamage;
        GestureEventEventPublisher.GestureEventEvent -= OnGesture;
        
        BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricade;
        
        SpawnDeadBodyEventPublisher.SpawnDeadBodyEvent -= SpawnBody;
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

        for (var i = storage.StorageItem!.Count - 1; i >= 0; i--)
        {
            var body = storage.StorageItem[i];
            if (CurrentAccessedInvs.ContainsValue(new Vector3(body.LocX, body.LocY, body.LocZ)) || (DateTime.Now - body.BodySpawnTime).TotalMilliseconds >= config.DespawnTime)
            {
                continue;
            }
            
            storage.StorageItem.RemoveAt(i);
            var trans = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(new Vector3(body.LocX, body.LocY, body.LocZ), 1, trans);

            if (trans.All(t => t.position != new Vector3(body.LocX, body.LocY, body.LocZ)))
            {
                continue;
            }

            var tran = trans.First(t => t.position == new Vector3(body.LocX, body.LocY, body.LocZ));

            var drop = BarricadeManager.FindBarricadeByRootTransform(tran);

            var man = drop.interactable as InteractableMannequin;
            man?.clearClothes();
            
            BarricadeManager.tryGetRegion(tran, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(drop, x, y, plant);
        }

        storage.Save();
    }

    private void OnDamageBarricade(CSteamID instigatorSteamId, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
    {
        if (!GetStorage<DeathsStorage>(out var storage))
        {
            return;
        }

        if (storage.InventoryAt(barricadeTransform.position))
        {
            shouldAllow = false;
        }
    }

    private void SpawnBody(SpawnDeadBodyEventArgs e) =>
        SendDeath(e.Player);

    private void OnGesture(GestureEventEventArgs e, ref bool allow)
    {
        if (e.Player == null)
        {
            return;
        }
        
        if (e.Gesture != EPlayerGesture.POINT)
        {
            return;
        }

        var result = Raycaster.RayCastPlayer(e.Player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return;
        }

        if (result.BarricadeRootTransform == null)
        {
            return;
        }

        if (Vector3.Distance(e.Player.Position, result.BarricadeRootTransform.position) > 2)
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

        if (CurrentAccessedInvs.ContainsValue(drop.model.position))
        {
            var user = CurrentAccessedInvs.First(x => x.Value == drop.model.position).Key;
            if (user.Item2 == user.Item1.Position && user.Item1.Inventory.isStoring)
            {
                UnturnedChat.Say(e.Player, "This body is currently being accessed.", Color.red);
                return;
            }

            CurrentAccessedInvs.Remove(user);
        }

        var inv = storage.GetInv(drop.model.position);

        CurrentAccessedInvs.Add((e.Player, e.Player.Position), drop.model.position);
        
        var inventory = new Items(7);
        inventory.resize(8, 100);

        foreach (var item in inv.Items)
        {
            inventory.tryAddItem(new SDG.Unturned.Item(item.ItemID, item.Amount, item.Quality, item.State));
        }

        inventory.onItemAdded = (_, _, jar) =>
        {
            var body = storage.GetInv(drop.model.position);
            storage.StorageItem?.Remove(body);
            body.Items.Add(new Item(jar.item.id, jar.item.amount, jar.item.quality, jar.item.state));
            storage.StorageItem?.Add(body);
            storage.Save();
        };

        inventory.onItemRemoved = (_, _, jar) =>
        {
            var body = storage.GetInv(drop.model.position);
            storage.StorageItem?.RemoveAll(b => new Vector3(b.LocX, b.LocY, b.LocZ) == drop.model.position);
            var item = body.Items.First(i => i.ItemID == jar.item.id);
            body.Items.Remove(item);
            storage.StorageItem?.Add(body);
            storage.Save();

            if (body.Items.Any())
            {
                return;
            }

            var man = drop.interactable as InteractableMannequin;
            man?.clearClothes();
            
            BarricadeManager.tryGetRegion(drop.model.transform, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(drop, x, y, plant);
        };
        
        e.Player.Inventory.isStoring = true;
        
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

    private void SaveInventory(Transform model, List<Item> playerItems)
    {
        var inv = new Body
        {
            BodySpawnTime = DateTime.Now,
            Items = playerItems,
            LocX = model!.position.x,
            LocY = model!.position.y,
            LocZ = model!.position.z
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

        var barriAngle = new Quaternion(0f, 0f, 0f, 0f);
        var barricade = new Barricade((ItemBarricadeAsset)Assets.find(EAssetType.ITEM, config.ManID));
        var transform = BarricadeManager.dropNonPlantedBarricade(barricade, player.Position, barriAngle, 0, 0);

        var barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(transform);
        return barricadeDrop;
    }

    private void SendDeath(UnturnedPlayer player)
    {
        var playerItems = new List<Item>();

        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                var item = player.Inventory.getItem(i, 0);
                playerItems.Add(new Item(item.item.id, item.item.amount, item.item.quality, item.item.state));
                player.Inventory.removeItem(i, 0);
            }
        }

        var barricadeDrop = PlaceBarricade(player);

        if (barricadeDrop.interactable as InteractableMannequin == null)
        {
            BarricadeManager.tryGetRegion(barricadeDrop.model, out var x, out var y, out var plant, out _);
            BarricadeManager.destroyBarricade(barricadeDrop, x, y, plant);
            return;
        }

        var man = barricadeDrop.interactable as InteractableMannequin;

        if (man == null)
        {
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

        SaveInventory(barricadeDrop.model, playerItems);
        RetroMedieval.Main.Instance.StartCoroutine(ClearInventoryCoroutine(player.SteamPlayer()));

        if (player.Player.life.IsAlive)
        {
            player.Player.life.sendSuicide();
        }
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

        yield break;
    }
}