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
                MainItem = 40515,
                Items =
                [
                    40517,
                    40516,
                    40518,
                    40519,
                    40520
                ]
            }
        ];
    }
}