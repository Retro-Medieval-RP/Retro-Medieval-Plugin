using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationWarns")]
internal class Warn() : ModerationAction(ModerationActionType.Warn)
{
    [DatabaseColumn("WarnRemoved", "TINYINT", "0")]
    public bool WarnRemoved { get; set; }
}