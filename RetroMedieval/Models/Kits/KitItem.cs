using Pathfinding.Util;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Kits;

[DatabaseTable("KitItems")]
internal class KitItem
{
    [DatabaseColumn("KitItemID", "CHAR(36)")]
    [PrimaryKey]
    public Guid KitItemID { get; set; }
    
    [DatabaseColumn("ItemID", "SMALLINT UNSIGNED")]
    public ushort ItemID { get; set; }
    
    [DatabaseColumn("ItemAmount", "INT")]
    public int ItemAmount { get; set; }
    
    [DatabaseColumn("ItemState", "BLOB")]
    public byte[] ItemState { get; set; }
    
    [DatabaseColumn("KitID", "CHAR(36)")]
    [ForeignKey(typeof(Kit), "KitID")]
    public Guid KitID { get; set; }
}