using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Kits.Models;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Kits;

[ModuleInformation("Kits")]
[ModuleStorage<MySqlSaver<Kit>>("KitsStorage")]
[ModuleStorage<MySqlSaver<KitItem>>("KitItemsStorage")]
internal class KitsModule([NotNull] string directory) : Module(directory)
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public void CreateKit(Kit kit, IEnumerable<KitItem> kitItems)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitItem>>(out var kitItemsStorage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        kitsStorage.StartQuery().Insert(kit).ExecuteSql();

        foreach (var item in kitItems)
        {
            item.KitID = kit.KitID;
            kitItemsStorage.StartQuery().Insert(item).ExecuteSql();
        }
    }

    public bool DoesKitExist(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return false;
        }

        var count = kitsStorage.StartQuery().Count().Where(("KitName", kitName)).Finalise().QuerySingle<int>();
        return count > 0;
    }

    public void RenameKit(string originalName, string newName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", originalName)).Finalise().QuerySingle<Guid>();
        kitsStorage.StartQuery().Update(("KitName", newName)).Where(("KitID", kitID)).Finalise().ExecuteSql();
    }

    public void DeleteKit(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        if (kitsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
        {
            Logger.LogError("Could not delete kit with id: " + kitID);
        }
    }

    private IEnumerable<Kit> GetKits()
    {
        if (GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            return kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Finalise().Query<Kit>();
        }

        Logger.LogError("Could not gather storage [KitsStorage]");
        return new List<Kit>();
    }

    public void SpawnKit(UnturnedPlayer targetPlayer, string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitItem>>(out var kitItemsStorage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        var kit = kitsStorage.StartQuery()
            .Select("KitID", "KitName", "KitCooldown")
            .Where(("KitName", kitName))
            .Finalise()
            .QuerySingle<Kit>();
        
        var kitItems = kitItemsStorage.StartQuery()
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
            .Query<KitItem>();

        foreach (var item in kitItems.OrderByDescending(x => x.IsEquipped))
        {
            if (!targetPlayer.Inventory.tryAddItem(new Item(item.ItemID, item.Amount, item.Quality, item.State), true,
                    true))
            {
                ItemManager.dropItem(new Item(item.ItemID, item.Amount, item.Quality, item.State),
                    targetPlayer.Position, false, true, true);
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