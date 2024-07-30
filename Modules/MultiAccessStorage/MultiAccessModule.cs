using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned.Storage;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace MultiAccessStorage;

[ModuleInformation("Multi User Storage")]
public class MultiAccessModule(string directory) : Module(directory)
{
    private Dictionary<Vector3, List<UnturnedPlayer>> UsersInChest { get; set; }
    private Dictionary<UnturnedPlayer, bool> TempCloses { get; set; }

    public override void Load()
    {
        UsersInChest = [];

        OpenStorageEventPublisher.OpenStorageEvent += OnOpenStorage;
        CloseStorageEventPublisher.CloseStorageEvent += OnCloseStorage;
    }

    private void OnOpenStorage(OpenStorageEventArgs e, ref bool allow)
    {
        if (e.StorageOpened == null)
        {
            return;
        }

        TempCloses.Add(e.Player, true);
        e.Player.Inventory.closeStorage();

        var inventory = new Items(7);
        inventory.resize(e.StorageOpened.items.width, e.StorageOpened.items.height);

        foreach (var item in e.StorageOpened.items.items)
        {
            inventory.addItem(item.x, item.y, item.rot, item.item);
        }

        inventory.onItemAdded = (_, _, jar) =>
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(e.StorageOpened.transform);
            var storage = drop.interactable as InteractableStorage;
            storage?.items.addItem(jar.x, jar.y, jar.rot, jar.item);

            var users = UsersInChest[e.StorageOpened.transform.position];
            foreach (var user in users)
            {
                user.Inventory.storage.items.addItem(jar.x, jar.y, jar.rot, jar.item);
            }
        };

        inventory.onItemRemoved = (_, index, _) =>
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(e.StorageOpened.transform);
            var storage = drop.interactable as InteractableStorage;
            storage?.items.removeItem(index);

            var users = UsersInChest[e.StorageOpened.transform.position];
            foreach (var user in users)
            {
                user.Inventory.storage.items.removeItem(index);
            }
        };
    }

    private void OnCloseStorage(CloseStorageEventArgs e, ref bool allow)
    {
        if (TempCloses.ContainsKey(e.Player))
        {
            TempCloses.Remove(e.Player);
            return;
        }

        if (!UsersInChest.TryGetValue(e.StorageClosed.transform.position, out var players))
        {
            return;
        }

        if (players.All(x => x.CSteamID.m_SteamID != e.Player.CSteamID.m_SteamID))
        {
            return;
        }

        Console.WriteLine($"Before | Number In Chest: {UsersInChest[e.StorageClosed.transform.position].Count}");
        UsersInChest[e.StorageClosed.transform.position]
            .RemoveAll(x => x.CSteamID.m_SteamID == e.Player.CSteamID.m_SteamID);
        Console.WriteLine($"After | Number In Chest: {UsersInChest[e.StorageClosed.transform.position].Count}");

        if (!UsersInChest[e.StorageClosed.transform.position].Any())
        {
            Console.WriteLine($"Removing Storage From UsersInChest");
            UsersInChest.Remove(e.StorageClosed.transform.position);
            Console.WriteLine($"Removed Storage From UsersInChest");
        }
    }

    public override void Unload()
    {
        UsersInChest.Clear();

        OpenStorageEventPublisher.OpenStorageEvent -= OnOpenStorage;
        CloseStorageEventPublisher.CloseStorageEvent -= OnCloseStorage;
    }
}