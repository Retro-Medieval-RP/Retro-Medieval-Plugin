using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Kits.Models;

[DatabaseTable("KitCooldowns")]
internal class KitCooldown
{
    [DatabaseColumn("CooldownID", "CHAR(36)")]
    [PrimaryKey]
    public Guid CooldownID { get; set; }
    
    [DatabaseColumn("KitID", "CHAR(36)")]
    [ForeignKey(typeof(Kit), "KitID")]
    public Guid KitID { get; set; }
    
    [DatabaseColumn("User", "BIGINT")]
    public ulong User { get; set; }
    
    [DatabaseColumn("SpawnDateTime", "DATETIME")]
    public DateTime SpawnDateTime { get; set; }
}