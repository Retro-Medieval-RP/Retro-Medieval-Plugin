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
        
        kits_storage.StartQuery().Insert(kit);
        foreach (var item in kit_items)
        {
            item.KitID = kit.KitID;
            kit_items_storage.StartQuery().Insert(item);
        }
    }
    
    public override void Unload()
    {
    }
}