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
[ModuleStorage<MySqlSaver<KitCooldown>>("KitCooldownsStorage")]
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
        var kits = GetKits();
        return kits.Any(x => string.Equals(x.KitName, kitName, StringComparison.CurrentCultureIgnoreCase));
    }

    public void RenameKit(string originalName, string newName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID", "KitName").Finalise().Query<Kit>()
            .First(x => x.KitName == originalName).KitID;
        kitsStorage.StartQuery().Update(("KitName", newName)).Where(("KitID", kitID)).Finalise().ExecuteSql();
    }

    public void DeleteKit(string kitName)
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

        var kitID = GetKits().First(x => x.KitName == kitName).KitID;
        if (!kitItemsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
        {
            return;
        }

        if (!kitsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
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
            .Finalise()
            .Query<Kit>()
            .First(x => x.KitName == kitName);

        var kitItems = kitItemsStorage.StartQuery()
            .Select("*")
            .Finalise()
            .Query<KitItem>()
            .Where(x => x.KitID == kit.KitID);

        foreach (var item in kitItems.OrderByDescending(x => x.IsEquipped))
        {
            if (!targetPlayer.Inventory.tryAddItem(
                    new Item(item.ItemID, (byte)item.ItemAmount, 100, item.ItemState), true,
                    true))
            {
                ItemManager.dropItem(new Item(item.ItemID, (byte)item.ItemAmount, 100, item.ItemState),
                    targetPlayer.Position, false, true, true);
            }
        }
    }

    public void SendKits(IRocketPlayer caller)
    {
        var kits = GetKits();
        UnturnedChat.Say(caller, "Kits:");
        UnturnedChat.Say(caller,
            string.Join(", ",
                kits
                    .Select(x => (x.KitName, x.CooldownString))
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.KitName) && !string.IsNullOrEmpty(x.KitName))
                    .Where(x => caller.HasPermission($"kit.{x.KitName}"))
                    .Select(x => $"{x.KitName} ({x.CooldownString})")));
    }

    public int GetCooldown(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return -1;
        }

        var kit = kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Where(("KitName", kitName))
            .Finalise().QuerySingle<Kit>();

        return kit.KitCooldown;
    }

    public Kit GetKit(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return null;
        }

        return kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Where(("KitName", kitName))
            .Finalise().QuerySingle<Kit>();
    }

    public bool IsKitOnCooldown(UnturnedPlayer targetPlayer, string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return true;
        }

        if (!GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return true;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        return kitCooldownsStorage.StartQuery().Count().Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingle<int>() > 0;
    }

    public DateTime GetLastSpawnDate(UnturnedPlayer targetPlayer, string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return DateTime.Now;
        }

        if (!GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return DateTime.Now;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        return kitCooldownsStorage.StartQuery().Select("SpawnDateTime").Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingle<DateTime>();
    }

    public void DeleteCooldown(UnturnedPlayer targetPlayer, string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        kitCooldownsStorage.StartQuery().Delete().Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().ExecuteSql();
    }

    public void AddCooldown(UnturnedPlayer targetPlayer, string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kitID = kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        kitCooldownsStorage.StartQuery().Insert(new KitCooldown
        {
            CooldownID = Guid.NewGuid(),
            KitID = kitID,
            SpawnDateTime = DateTime.Now,
            User = targetPlayer.CSteamID.m_SteamID
        }).ExecuteSql();
    }
}