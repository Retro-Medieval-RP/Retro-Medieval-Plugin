using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Kits;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
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

        var count = kits_storage.StartQuery().Count().Where(("KitName", kit_name)).Finalise().QuerySql<int>();
        return count > 0;
    }

    public void RenameKit(string original_name, string new_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit_id = kits_storage.StartQuery().Select("KitID").Where(("KitName", original_name)).Finalise().QuerySql<Guid>();
        kits_storage.StartQuery().Update(("KitName", new_name)).Where(("KitID", kit_id)).Finalise().ExecuteSql();
    }

    public void DeleteKit(string kit_name)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit_id = kits_storage.StartQuery().Select("KitID").Where(("KitName", kit_name)).Finalise().QuerySql<Guid>();
        if (kits_storage.StartQuery().Delete().Where(("KitID", kit_id)).Finalise().ExecuteSql())
        {
            Logger.LogError("Could not delete kit with id: " + kit_id);
        }
    }

    private IEnumerable<Kit> GetKits()
    {
        if (GetStorage<MySqlSaver<Kit>>(out var kits_storage))
        {
            return kits_storage.StartQuery().Select("KitID", "KitName", "KitCooldown").Finalise().QuerySql<IEnumerable<Kit>>();
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

        var kit = kits_storage.StartQuery()
            .Select("KitID", "KitName", "KitCooldown")
            .Where(("KitName", kit_name))
            .Finalise()
            .QuerySql<Kit>();
        var kit_items = kit_items_storage.StartQuery()
            .Select(
                "KitItemID",
                "IsEquipped",
                "KitID",
                "ItemID",
                "ItemAmount",
                "ItemQuality",
                "ItemState"
            )
            .Where(("KitID", kit.KitID))
            .Finalise()
            .QuerySql<IEnumerable<KitItem>>();

        foreach (var item in kit_items.OrderByDescending(x => x.IsEquipped))
        {
            if (!target_player.Inventory.tryAddItem(new Item(item.ItemID, item.Amount, item.Quality, item.State), true,
                    true))
            {
                ItemManager.dropItem(new Item(item.ItemID, item.Amount, item.Quality, item.State),
                    target_player.Position, false, true, true);
            }
        }
    }

    public void SendKits(IRocketPlayer caller)
    {
        var kits = GetKits();
        UnturnedChat.Say(caller, "Kits:");
        UnturnedChat.Say(caller,
            string.Join(", ", kits.Select(x => x.KitName).Where(x => caller.HasPermission($"kit.{x}"))));
    }
}