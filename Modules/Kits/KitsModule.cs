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
using Rocket.Unturned.Player;

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

    public void CreateKit(Kit kit, IEnumerable<KitItem> kitItems) => 
        ThreadCalls.CreateKit.Start(new Tuple<KitsModule, Kit, IEnumerable<KitItem>>(this, kit, kitItems));

    public void RenameKit(string originalName, string newName) => 
        ThreadCalls.RenameKit.Start(new Tuple<KitsModule, string, string>(this, originalName, newName));

    public void DeleteKit(string kitName) => 
        ThreadCalls.DeleteKit.Start(new Tuple<KitsModule, string>(this, kitName));

    public void SpawnKit(UnturnedPlayer targetPlayer, string kitName) => 
        ThreadCalls.SpawnKit.Start(new Tuple<KitsModule, UnturnedPlayer, string>(this, targetPlayer, kitName));

    public void SendKits(IRocketPlayer caller) => 
        ThreadCalls.SendKits.Start(new Tuple<KitsModule, IRocketPlayer>(this, caller));

    public async Task<bool> DoesKitExist(string kitName)
    {
        var kits = await GetKits();
        return kits.Any(x => string.Equals(x.KitName, kitName, StringComparison.CurrentCultureIgnoreCase));
    }

    internal async Task<IEnumerable<Kit>> GetKits()
    {
        if (GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            return await kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Finalise().QueryAsync<Kit>();
        }

        Logger.LogError("Could not gather storage [KitsStorage]");
        return new List<Kit>();
    }

    public async Task<int> GetCooldown(string kitName)
    {
        if (!GetStorage<MySqlSaver<Kit>>(out var kitsStorage))
        {
            Logger.LogError("Could not gather storage [KitsStorage]");
            return -1;
        }

        var kit = await kitsStorage.StartQuery().Select("KitID", "KitName", "KitCooldown").Where(("KitName", kitName))
            .Finalise().QuerySingleAsync<Kit>();

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
            .Finalise().QuerySingleAsync<Kit>();
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingleAsync<Guid>();
        return await kitCooldownsStorage.StartQuery().Count().Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingleAsync<int>() > 0;
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

        var kitID = await kitsStorage.StartQuery().Select("KitID").Where(("KitName", kitName)).Finalise().QuerySingleAsync<Guid>();
        return await kitCooldownsStorage.StartQuery().Select("SpawnDateTime").Where(("KitID", kitID), ("User", targetPlayer.CSteamID.m_SteamID)).Finalise().QuerySingleAsync<DateTime>();
    }

    public void DeleteCooldown(UnturnedPlayer targetPlayer, string kitName) => 
        ThreadCalls.DeleteCooldown.Start(new Tuple<KitsModule, UnturnedPlayer, string>(this, targetPlayer, kitName));

    public void AddCooldown(UnturnedPlayer targetPlayer, string kitName) => 
        ThreadCalls.AddCooldown.Start(new Tuple<KitsModule, UnturnedPlayer, string>(this, targetPlayer, kitName));
}