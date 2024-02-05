using System.Collections.Generic;
using RetroMedieval.Models.ArmorEquip;
using RetroMedieval.Modules.Configuration;

namespace RetroMedieval.Modules.ArmorEquip;

internal class ArmorEquipConfiguration : IConfig
{
    public List<ArmorSet> ArmorSets { get; set; }

    public void LoadDefaults()
    {
        ArmorSets =
        [
            new ArmorSet
            {
                ReplaceClothing = true,
                DropInventoryWhenEquip = true,
                MainItem = 40517,
                Items =
                [
                    40515,
                    40516,
                    40518,
                    40519,
                    40520
                ]
            }
        ];
    }
}