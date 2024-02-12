using System;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql.StatementsAndQueries;

namespace RetroMedieval.Modules.Kits;

[ModuleInformation("Kits")]
[ModuleStorage<KitsStorage>("KitsStorage")]
[ModuleStorage<KitItemsStorage>("KitItemsStorage")]
internal class KitsModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }
}