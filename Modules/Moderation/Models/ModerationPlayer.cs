using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Moderation.Models;

[DatabaseTable("Players")]
internal class ModerationPlayer
{
    [DatabaseColumn("PlayerID", "BIGINT")]
    [PrimaryKey]
    public ulong PlayerID { get; set; }

    [DatabaseColumn("DisplayName", "VARCHAR(255)")]
    public string DisplayName { get; set; }

    [DatabaseColumn("LastJoinDate", "DATETIME")]
    public DateTime LastJoinDate { get; set; }

    [DatabaseColumn("FirstJoinDate", "DATETIME")]
    public DateTime FirstJoinDate { get; set; }
}