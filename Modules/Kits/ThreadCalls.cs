using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kits.Models;
using RetroMedieval.Savers.MySql;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Kits;

internal static class ThreadCalls
{
    internal static Thread CreateKit = new(CreateKitStart);
    internal static Thread RenameKit = new(RenameKitStart);
    internal static Thread DeleteKit = new(DeleteKitStart);
    internal static Thread SpawnKit = new(SpawnKitStart);
    internal static Thread SendKits = new(SendKitsStart);

    internal static Thread DeleteCooldown = new(DeleteCooldownStart);
    internal static Thread AddCooldown = new(AddCooldownStart);

    private static void CreateKitStart(object obj)
    {
        var v = obj as Tuple<KitsModule, Kit, IEnumerable<KitItem>>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!v.Item1.GetStorage<MySqlSaver<KitItem>>(out var kitItemsStorage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        kitsStorage.StartQuery().Insert(v.Item2).ExecuteSql();

        foreach (var item in v.Item3)
        {
            item.KitID = v.Item2.KitID;
            kitItemsStorage.StartQuery().Insert(item).ExecuteSql();
        }
    }

    private static async void RenameKitStart(object obj)
    {
        var v = obj as Tuple<KitsModule, string, string>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        var kit = await kitsStorage.StartQuery().Select("KitID", "KitName").Where(("KitName", v.Item2)).Finalise()
            .QuerySingle<Kit>();
        var kitId = kit.KitID;
        kitsStorage.StartQuery().Update(("KitName", v.Item3)).Where(("KitID", kitId)).Finalise().ExecuteSql();
    }

    private static async void DeleteKitStart(object obj)
    {
        var v = obj as Tuple<KitsModule, string>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!v.Item1.GetStorage<MySqlSaver<KitItem>>(out var kitItemsStorage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        if (!v.Item1.GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kits = await v.Item1.GetKits();
        var kitID = kits.First(x => x.KitName == v.Item2).KitID;
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

    private static async void SpawnKitStart(object obj)
    {
        var v = obj as Tuple<KitsModule, UnturnedPlayer, string>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!v.Item1.GetStorage<MySqlSaver<KitItem>>(out var kitItemsStorage))
        {
            Logger.LogError("Could not gather storage [KitItemsStorage]");
            return;
        }

        var kit = await kitsStorage.StartQuery()
            .Select("KitID", "KitName", "KitCooldown")
            .Where(("KitName", v.Item3))
            .Finalise()
            .QuerySingle<Kit>();

        var kitItems = await kitItemsStorage.StartQuery()
            .Select("*")
            .Where(("KitID", kit.KitID))
            .Finalise()
            .Query<KitItem>();

        foreach (var item in kitItems.OrderByDescending(x => x.IsEquipped))
        {
            if (!v.Item2.Inventory.tryAddItem(
                    new Item(item.ItemID, (byte)item.ItemAmount, 100, item.ItemState), true,
                    true))
            {
                ItemManager.dropItem(new Item(item.ItemID, (byte)item.ItemAmount, 100, item.ItemState),
                    v.Item2.Position, false, true, true);
            }
        }
    }

    private static async void SendKitsStart(object obj)
    {
        var v = obj as Tuple<KitsModule, IRocketPlayer>;
        var kits = await v!.Item1.GetKits();
        UnturnedChat.Say(v.Item2, "Kits:");
        UnturnedChat.Say(v.Item2,
            string.Join(", ",
                kits
                    .Select(x => (x.KitName, x.CooldownString))
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.KitName) && !string.IsNullOrEmpty(x.KitName))
                    .Where(x => v.Item2.HasPermission($"kit.{x.KitName}"))
                    .Select(x => $"{x.KitName} ({x.CooldownString})")));
    }

    private static async void DeleteCooldownStart(object obj)
    {
        var v = obj as Tuple<KitsModule, UnturnedPlayer, string>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!v.Item1.GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", v.Item3)).Finalise()
            .QuerySingle<Guid>();
        
        kitCooldownsStorage.StartQuery().Delete().Where(("KitID", kitID), ("User", v.Item2.CSteamID.m_SteamID))
            .Finalise().ExecuteSql();
    }

    private static async void AddCooldownStart(object obj)
    {
        var v = obj as Tuple<KitsModule, UnturnedPlayer, string>;
        if (!v!.Item1.GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return;
        }

        if (!v!.Item1.GetStorage<MySqlSaver<KitCooldown>>(out var kitCooldownsStorage))
        {
            Logger.LogError("Could not gather storage [KitCooldownsStorage]");
            return;
        }

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", v!.Item3)).Finalise()
            .QuerySingle<Guid>();
        
        kitCooldownsStorage.StartQuery().Insert(new KitCooldown
        {
            CooldownID = Guid.NewGuid(),
            KitID = kitID,
            SpawnDateTime = DateTime.Now,
            User = v!.Item2.CSteamID.m_SteamID
        }).ExecuteSql();
    }
}