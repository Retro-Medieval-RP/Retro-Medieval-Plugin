using System.Collections.Generic;
using System.Linq;
using ArmorEquip.Models;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned.CloathingDequip;
using RetroMedieval.Shared.Events.Unturned.ClothingEquip;
using RetroMedieval.Shared.Events.Unturned.Inventory;
using RetroMedieval.Shared.Events.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace ArmorEquip;

[ModuleInformation("ArmorEquip")]
[ModuleConfiguration<ArmorEquipConfiguration>("Armors")]
internal class ArmorEquipModule([NotNull] string directory) : Module(directory)
{
    private Dictionary<CSteamID, bool> IgnoreEquipOfClothing { get; } = [];
    private Dictionary<CSteamID, bool> IgnoreDequipOfClothing { get; } = [];
    private Dictionary<CSteamID, bool> RemoveItem { get; } = [];
    
    public override void Load()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent += OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent += OnClothingDequipped;

        ItemAddEventPublisher.ItemAddEvent += OnItemGive;
    }

    public override void Unload()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent -= OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent -= OnClothingDequipped;

        ItemAddEventPublisher.ItemAddEvent -= OnItemGive;
    }

    private void OnItemGive(ItemAddEventArgs e, ref bool allow)
    {
        if (RemoveItem.ContainsKey(e.Player.CSteamID))
        {
            allow = false;
            RemoveItem.Remove(e.Player.CSteamID);
        }
    }

    private void OnClothingDequipped(ClothingDequipEventArgs e, ref bool allow)
    {
        if (IgnoreDequipOfClothing.ContainsKey(e.Player.CSteamID))
        {
            return;
        }

        if (!GetConfiguration<ArmorEquipConfiguration>(out var configuration))
        {
            return;
        }

        if (!configuration.ArmorSets.Any(x =>
                x.MainItem == e.DequippedClothingID || x.Items.Contains(e.DequippedClothingID)))
        {
            return;
        }

        {
            var set = configuration.ArmorSets.First(x =>
                x.MainItem == e.DequippedClothingID || x.Items.Contains(e.DequippedClothingID));

            if (e.DequippedClothingID != set.MainItem)
            {
                allow = false;
                RemoveItem.Add(e.Player.CSteamID, true);
                return;
            }

            var items = GetUserItems(e.Player);

            foreach (var item in items)
            {
                ItemManager.dropItem(new Item(item.ItemID, item.ItemAmount, item.ItemQuality, item.ItemState),
                    e.Player.Position, true, true, true);
            }

            ClearUserItems(e.Player);

            IgnoreDequipOfClothing.Add(e.Player.CSteamID, true);
        }
    }

    private void OnClothingEquipped(ClothingEquipEventArgs e, ref bool allow)
    {
        if (IgnoreEquipOfClothing.ContainsKey(e.Player.CSteamID))
        {
            return;
        }

        if (!GetConfiguration<ArmorEquipConfiguration>(out var configuration))
        {
            return;
        }

        if (configuration.ArmorSets.All(x => x.MainItem != e.ClothingItem))
        {
            return;
        }

        var set = configuration.ArmorSets.Find(x => x.MainItem == e.ClothingItem);
        var items = GetUserItems(e.Player);

        if (set.DropInventoryWhenEquip)
        {
            foreach (var item in items)
            {
                ItemManager.dropItem(new Item(item.ItemID, item.ItemAmount, item.ItemQuality, item.ItemState),
                    e.Player.Position, true, true, true);
            }

            ClearUserItems(e.Player);
        }

        IgnoreEquipOfClothing.Add(e.Player.CSteamID, true);

        foreach (var item in set.Items)
        {
            var asset = Assets.find(EAssetType.ITEM, item) as ItemAsset;

            switch (asset!.type)
            {
                case EItemType.HAT:
                {
                    if (e.Player.Player.clothing.hat != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.hat, 1, e.Player.Player.clothing.hatQuality,
                                e.Player.Player.clothing.hatState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearHat(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.SHIRT:
                {
                    if (e.Player.Player.clothing.shirt != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.shirt, 1, e.Player.Player.clothing.shirtQuality,
                                e.Player.Player.clothing.shirtState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearShirt(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.PANTS:
                {
                    if (e.Player.Player.clothing.pants != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.pants, 1, e.Player.Player.clothing.pantsQuality,
                                e.Player.Player.clothing.pantsState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearPants(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.BACKPACK:
                {
                    if (e.Player.Player.clothing.backpack != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.backpack, 1, e.Player.Player.clothing.backpackQuality,
                                e.Player.Player.clothing.backpackState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearBackpack(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.VEST:
                {
                    if (e.Player.Player.clothing.vest != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.vest, 1, e.Player.Player.clothing.vestQuality,
                                e.Player.Player.clothing.vestState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearVest(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.MASK:
                {
                    if (e.Player.Player.clothing.mask != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.mask, 1, e.Player.Player.clothing.maskQuality,
                                e.Player.Player.clothing.maskState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearVest(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                case EItemType.GLASSES:
                {
                    if (e.Player.Player.clothing.glasses != 0)
                    {
                        ItemManager.dropItem(
                            new Item(e.Player.Player.clothing.glasses, 1, e.Player.Player.clothing.glassesQuality,
                                e.Player.Player.clothing.glassesState), e.Player.Position, true, true, true);
                        e.Player.Player.clothing.askWearVest(0, 0, [], true);
                        Action();
                    }

                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
                default:
                {
                    e.Player.Player.inventory.tryAddItemAuto(new Item(item, true), true, true, true, true);
                    break;
                }
            }
        }

        IgnoreEquipOfClothing.Remove(e.Player.CSteamID);

        return;

        void Action()
        {
            for (byte index = 0; index < e.Player.Inventory.getItemCount(2); ++index)
            {
                e.Player.Inventory.removeItem(2, 0);
            }
        }
    }

    private static List<UserItem> GetUserItems(UnturnedPlayer player)
    {
        var userItems = new List<UserItem>();

        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                var item = player.Inventory.getItem(i, index);
                userItems.Add(new UserItem
                {
                    ItemID = item.item.id,
                    ItemState = item.item.state,
                    ItemAmount = item.item.amount,
                    ItemQuality = item.item.quality,
                });
            }
        }

        return userItems;
    }

    private static void ClearUserItems(UnturnedPlayer player)
    {
        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                player.Inventory.removeItem(i, 0);
            }
        }
    }
}