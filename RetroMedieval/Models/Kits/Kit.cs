using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Kits;


[DatabaseTable("Kits")]
internal class Kit
{
    [DatabaseColumn("KitID", "CHAR(36)")]
    [PrimaryKey]
    public Guid KitID { get; set; }

    [DatabaseColumn("KitName", "VARCHAR(255)")]
    public string KitName { get; set; }

    [DatabaseColumn("KitCooldown", "INT", "-1")]
    public int Cooldown { get; set; }
}