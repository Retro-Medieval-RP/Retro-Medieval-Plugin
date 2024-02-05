using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Events.Unturned.Clothing;
using RetroMedieval.Modules.Attributes;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace RetroMedieval.Modules.ArmorEquip;

[ModuleInformation("ArmorEquip")]
[ModuleConfiguration<ArmorEquipConfiguration>("Armors")]
internal class ArmorEquipModule : Module
{
    private List<CSteamID> IgnoredPlayers { get; set; } = [];

    public override void Load()
    {
        ClothingEquipEventPublisher.ClothingEquipEvent += OnClothingEquipped;
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
            e.Player.Player.clothing.askWearBackpack(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearGlasses(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearHat(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearPants(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearMask(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearShirt(0, 0, Array.Empty<byte>(), true);
            e.Player.Player.clothing.askWearVest(0, 0, Array.Empty<byte>(), true);

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

        foreach (var item in set.Items)
        {
            Equip(item, e.Player);
        }
    }

    private static void Equip(ushort item, UnturnedPlayer player) =>
        player.Inventory.forceAddItem(new Item(item, true), true, true);

    public override void Unload()
    {
    }
}