using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<bool> DoesKitExist(string kitName)
    {
        var kits = await GetKits();
        return kits.Any(x => string.Equals(x.KitName, kitName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task RenameKit(string originalName, string newName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit = await kitsStorage.StartQuery().Select("KitID", "KitName").Where(("KitName", originalName)).Finalise().QuerySingle<Kit>();
        var kitId = kit.KitID;
        kitsStorage.StartQuery().Update(("KitName", newName)).Where(("KitID", kitId)).Finalise().ExecuteSql();
    }

    public async Task DeleteKit(string kitName)
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

        if (!GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kits = await GetKits();
        var kitID = kits.First(x => x.KitName == kitName).KitID;
        if (!kitItemsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
        {
            return;
        }

        if (!kitCooldownsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
        {
            return;
        }

        if (!kitsStorage.StartQuery().Delete().Where(("KitID", kitID)).Finalise().ExecuteSql())
        {
            Logger.LogError("Could not delete kit with id: " + kitID);
        }
    }

    private async Task<IEnumerable<Kit>> GetKits()
    {
        if (GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            return await kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Finalise().Query<Kit>();
        }

        Logger.LogError("Could not gather storage [KitsStorage]");
        return new List<Kit>();
    }

    public async Task SpawnKit(UnturnedPlayer targetPlayer, string kitName)
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

        var kit = await kitsStorage.StartQuery()
            .Select("KitID", "KitName", "KitCooldown")
            .Where(("KitName", kitName))
            .Finalise()
            .QuerySingle<Kit>();

        var kitItems = await kitItemsStorage.StartQuery()
            .Select("*")
            .Where(("KitID", kit.KitID))
            .Finalise()
            .Query<KitItem>();

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

    public async Task SendKits(IRocketPlayer caller)
    {
        var kits = await GetKits();
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

    public async Task<int> GetCooldown(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return -1;
        }

        var kit = await kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Where(("KitName", kitName))
            .Finalise().QuerySingle<Kit>();

        return kit.KitCooldown;
    }

    public async Task<Kit> GetKit(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return null;
        }

        return await kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Where(("KitName", kitName))
            .Finalise().QuerySingle<Kit>();
    }

    public async Task<bool> IsKitOnCooldown(UnturnedPlayer targetPlayer, string kitName)
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        return await kitCooldownsStorage.StartQuery().Count().Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingle<int>() > 0;
    }

    public async Task<DateTime> GetLastSpawnDate(UnturnedPlayer targetPlayer, string kitName)
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        return await kitCooldownsStorage.StartQuery().Select("SpawnDateTime").Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingle<DateTime>();
    }

    public async Task DeleteCooldown(UnturnedPlayer targetPlayer, string kitName)
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        kitCooldownsStorage.StartQuery().Delete().Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().ExecuteSql();
    }

    public async Task AddCooldown(UnturnedPlayer targetPlayer, string kitName)
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingle<Guid>();
        kitCooldownsStorage.StartQuery().Insert(new KitCooldown
        {
            CooldownID = Guid.NewGuid(),
            KitID = kitID,
            SpawnDateTime = DateTime.Now,
            User = targetPlayer.CSteamID.m_SteamID
        }).ExecuteSql();
    }
}