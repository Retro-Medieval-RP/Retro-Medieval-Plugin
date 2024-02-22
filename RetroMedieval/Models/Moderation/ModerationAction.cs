using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

internal class ModerationAction
{
    [DatabaseColumn("PunishmentID", "CHAR(36)")]
    [PrimaryKey]
    public Guid PunishmentID { get; set; }

    [DatabaseColumn("PunisherID", "")] 
    public ulong PunisherID { get; set; }

    [DatabaseColumn("TargetID", "")] 
    public ulong TargetID { get; set; }
    
    [DatabaseColumn("PunishmentGiven", "DATETIME", "")]
    public DateTime PunishmentGiven { get; set; }

    [DatabaseColumn("Reason", "VARCHAR(255)", "No Reason Specified")]
    public string Reason { get; set; }
}