using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Kits;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using Rocket.Core.Logging;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Modules.Kits;

[ModuleInformation("Kits")]
[ModuleStorage<MySqlSaver<Kit>>("KitsStorage")]
[ModuleStorage<MySqlSaver<KitItem>>("KitItemsStorage")]
internal class KitsModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public void CreateKit(Kit kit, IEnumerable<KitItem> kit_items)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitItem>>(out var kit_items_storage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        kits_storage.StartQuery().Insert(kit).ExecuteSql();

        foreach (var item in kit_items)
        {
            item.KitID = kit.KitID;
            kit_items_storage.StartQuery().Insert(item).ExecuteSql();
        }
    }

    public bool DoesKitExist(string kit_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return false;
        }

        var count = kits_storage.StartQuery().Count(("KitName", kit_name)).QuerySql<int>();
        return count > 0;
    }

    public void RenameKit(string original_name, string new_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit_id = kits_storage.StartQuery().Where(("KitName", original_name)).Select("KitID").QuerySql<Guid>();
        kits_storage.StartQuery().Where(("KitID", kit_id)).Update(("KitName", new_name)).ExecuteSql();
    }

    public void DeleteKit(string kit_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit_id = kits_storage.StartQuery().Where(("KitName", kit_name)).Select("KitID").QuerySql<Guid>();
        kits_storage.StartQuery().Where(("KitID", kit_id)).Delete().ExecuteSql();
    }

    public IEnumerable<Kit> GetKits()
    {
        if (GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            return kits_storage.StartQuery().Select("KitID", "KitName", "KitCooldown").QuerySql<IEnumerable<Kit>>();
        }

        Logger.LogError("Could not gather storage [KitsStorage]");
        return new List<Kit>();
    }

    public void SpawnKit(UnturnedPlayer target_player, string kit_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitItem>>(out var kit_items_storage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        var kit = kits_storage.StartQuery().Where(("KitName", kit_name)).Select("KitID", "KitName", "KitCooldown")
            .QuerySql<Kit>();
        var kit_items = kit_items_storage.StartQuery().Where(("KitID", kit.KitID)).Select(
            "KitItemID",
            "IsEquipped",
            "KitID",
            "ItemID",
            "ItemAmount",
            "ItemQuality",
            "ItemState"
        ).QuerySql<IEnumerable<KitItem>>();

        foreach (var item in kit_items.OrderByDescending(x => x.IsEquipped))
        {
            if (!target_player.Inventory.tryAddItem(new Item(item.ItemID, item.Amount, item.Quality, item.State), true, true))
            {
                ItemManager.dropItem(new Item(item.ItemID, item.Amount, item.Quality, item.State), target_player.Position, false, true, true);
            }
        }
    }
}