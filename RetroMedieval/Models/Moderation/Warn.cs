using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationWarns")]
internal class Warn : ModerationAction
{
}