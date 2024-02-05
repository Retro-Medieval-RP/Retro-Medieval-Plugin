using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Events.Unturned.Clothing;
using RetroMedieval.Modules.Attributes;
using Rocket.Core.Logging;
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

        if (config.ArmorSets.Any(x => x.MainItem == e.ClothingItem))
        {
            var set = config.ArmorSets.Find(x => x.MainItem == e.ClothingItem);

            if (set.DropInventoryWhenEquip)
            {
                for (byte i = 0; i < PlayerInventory.PAGES; i++)
                {
                    if (i == PlayerInventory.AREA)
                        continue;

                    var count = e.Player.Inventory.getItemCount(i);

                    for (byte index = 0; index < count; index++)
                    {
                        var item = e.Player.Inventory.getItem(i, 0);
                        ItemManager.dropItem(new Item(item.item.id, item.item.amount, item.item.quality, item.item.state), e.Player.Position, true, true, false);
                        e.Player.Inventory.removeItem(i, 0);
                    }
                }
            }
        }
    }

    public override void Unload()
    {
    }
}