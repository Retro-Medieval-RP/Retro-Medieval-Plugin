using Pathfinding.Util;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Kits;

[DatabaseTable("KitItems")]
internal class KitItem : Item
{
    [DatabaseColumn("KitItemID", "CHAR(36)")]
    [PrimaryKey]
    public Guid KitItemID { get; set; }
    
    [DatabaseColumn("IsEquipped", "TINYINT(1)")]
    public bool IsEquipped { get; set; }
    
    [DatabaseColumn("KitID", "CHAR(36)")]
    [ForeignKey(typeof(Kit), "KitID")]
    public Guid KitID { get; set; }
}