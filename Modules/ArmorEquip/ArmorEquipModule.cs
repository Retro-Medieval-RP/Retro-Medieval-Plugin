using System.Collections.Generic;
using System.Linq;
using ArmorEquip.Models;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned.CloathingDequip;
using RetroMedieval.Shared.Events.Unturned.ClothingEquip;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace ArmorEquip;

[ModuleInformation("ArmorEquip")]
[ModuleConfiguration<ArmorEquipConfiguration>("Armors")]
internal class ArmorEquipModule([NotNull] string directory) : Module(directory)
{
    private Dictionary<CSteamID, bool> IgnoreEquipOfClothing { get; } = [];
    
    public override void Load()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent += OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent += OnClothingDequipped;
    }

    private void OnClothingDequipped(ClothingDequipEventArgs e)
    {
    }

    private void OnClothingEquipped(ClothingEquipEventArgs e)
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
                ItemManager.dropItem(new Item(item.ItemID, item.ItemAmount, item.ItemQuality, item.ItemState), e.Player.Position, true, true, true);
            }

            ClearUserItems(e.Player);
        }
        
        IgnoreEquipOfClothing.Add(e.Player.CSteamID, true);
        // TODO: Add in the checks for if the user has the clothing equipped, if yes remove it for that slot, drop it on the floor and remove it from the inv and replace with set equipment
    }

    private static void Equip(ushort item, UnturnedPlayer player) =>
        player.Inventory.forceAddItem(new Item(item, true), true, true);

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
    
    public override void Unload()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent -= OnClothingEquipped;
        ClothingDequipEventPublisher.ClothingDequipEvent -= OnClothingDequipped;
    }
}