using System;
using System.Collections.Generic;
using System.Linq;
using ArmorEquip.Models;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.CloathingDequip;
using RetroMedieval.Shared.Events.ClothingEquip;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace ArmorEquip;

[ModuleInformation("ArmorEquip")]
[ModuleConfiguration<ArmorEquipConfiguration>("Armors")]
internal class ArmorEquipModule([NotNull] string directory) : Module(directory)
{
    private List<CSteamID> IgnoredPlayers { get; set; } = [];
    private Dictionary<CSteamID, ArmorSet> PlayerSets { get; set; } = [];

    public override void Load()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent += OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent += OnClothingDequipped;
    }

    private void OnClothingDequipped(ClothingDequipEventArgs e)
    {
        if (!PlayerSets.ContainsKey(e.Player.CSteamID) || IgnoredPlayers.Contains(e.Player.CSteamID))
        {
            return;
        }

        var set = PlayerSets[e.Player.CSteamID];

        if (!set.Items.Contains(e.DequippedClothingID) && set.MainItem != e.DequippedClothingID)
        {
            return;
        }
        
        ItemManager.dropItem(new Item(set.MainItem, true), e.Player.Position, true, true, false);
        
        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = e.Player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                var item = e.Player.Inventory.getItem(i, 0);
                ItemManager.dropItem(new Item(item.item.id, item.item.amount, item.item.quality, item.item.state),
                    e.Player.Position, true, true, false);
                e.Player.Inventory.removeItem(i, 0);
            }
        }
        
        var removeUnequipped = () =>
        {
            for (byte i = 0; i < e.Player.Player.inventory.getItemCount(2); i++)
            {
                e.Player.Player.inventory.removeItem(2, 0);
            }
        };
        
        IgnoredPlayers.Add(e.Player.CSteamID);
        
        e.Player.Player.clothing.askWearBackpack(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearGlasses(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearHat(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearPants(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearMask(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearShirt(0, 0, [], true);
        removeUnequipped();
        e.Player.Player.clothing.askWearVest(0, 0, [], true);
        removeUnequipped();
        
        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = e.Player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                e.Player.Inventory.removeItem(i, 0);
            }
        }
        
        IgnoredPlayers.Remove(e.Player.CSteamID);
        PlayerSets.Remove(e.Player.CSteamID);
    }

    private void OnClothingEquipped(ClothingEquipEventArgs e)
    {
        if (IgnoredPlayers.Contains(e.Player.CSteamID))
        {
            return;
        }

        if (!GetConfiguration<ArmorEquipConfiguration>(out var config))
        {
            Logger.LogError("Could not gather configuration [ArmorEquipConfiguration]");
            return;
        }

        if (config.ArmorSets.All(x => x.MainItem != e.ClothingItem))
        {
            return;
        }

        var set = config.ArmorSets.Find(x => x.MainItem == e.ClothingItem);

        if (set.DropInventoryWhenEquip)
        {
            e.Player.Player.clothing.askWearBackpack(0, 0, [], true);
            e.Player.Player.clothing.askWearGlasses(0, 0, [], true);
            e.Player.Player.clothing.askWearHat(0, 0, [], true);
            e.Player.Player.clothing.askWearPants(0, 0, [], true);
            e.Player.Player.clothing.askWearMask(0, 0, [], true);
            e.Player.Player.clothing.askWearShirt(0, 0, [], true);
            e.Player.Player.clothing.askWearVest(0, 0, [], true);

            for (byte i = 0; i < PlayerInventory.PAGES; i++)
            {
                if (i == PlayerInventory.AREA)
                    continue;

                var count = e.Player.Inventory.getItemCount(i);

                for (byte index = 0; index < count; index++)
                {
                    var item = e.Player.Inventory.getItem(i, 0);
                    ItemManager.dropItem(new Item(item.item.id, item.item.amount, item.item.quality, item.item.state),
                        e.Player.Position, true, true, false);
                    e.Player.Inventory.removeItem(i, 0);
                }
            }
        }
        
        IgnoredPlayers.Add(e.Player.CSteamID);
        foreach (var item in set.Items)
        {
            Equip(item, e.Player);
        }
        
        IgnoredPlayers.Remove(e.Player.CSteamID);
        PlayerSets.Add(e.Player.CSteamID, set);
    }

    private static void Equip(ushort item, UnturnedPlayer player) =>
        player.Inventory.forceAddItem(new Item(item, true), true, true);

    public override void Unload()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent -= OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent -= OnClothingDequipped;
    }
}