using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Kits.Models;

[DatabaseTable("KitItems")]
internal class KitItem
{
    [DatabaseColumn("KitItemID", "CHAR(36)")]
    [PrimaryKey]
    public Guid KitItemID { get; set; }
    
    [DatabaseColumn("KitID", "CHAR(36)")]
    [ForeignKey(typeof(Kit), "KitID")]
    public Guid KitID { get; set; }
    
    [DatabaseColumn("ItemID", "SMALLINT UNSIGNED")]
    public ushort ItemID { get; set; }
    
    [DatabaseColumn("IsEquipped", "TINYINT(1)")]
    public bool IsEquipped { get; set; }

    [DatabaseColumn("ItemAmount", "INT")]
    public int ItemAmount { get; set; }

    [DatabaseColumn("ItemQuality", "TINYINT UNSIGNED")]
    public byte ItemQuality { get; set; }

    [DatabaseColumn("ItemState", "BLOB")]
    public byte[] ItemState { get; set; }
}