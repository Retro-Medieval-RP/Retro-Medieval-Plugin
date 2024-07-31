using System.Collections.Generic;
using ArmorEquip.Models;
using RetroMedieval.Modules.Configuration;

namespace ArmorEquip;

internal class ArmorEquipConfiguration : IConfig
{
    public List<ArmorSet> ArmorSets { get; set; }

    public void LoadDefaults()
    {
        ArmorSets =
        [
            new ArmorSet
            {
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