using System;
using System.Collections.Generic;
using RetroMedieval.Models.Kits;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.StatementsAndQueries;
using Rocket.Core.Logging;

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
        
        if(!GetStorage<MySqlSaver<KitItem>>(out var kit_items_storage))
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
}