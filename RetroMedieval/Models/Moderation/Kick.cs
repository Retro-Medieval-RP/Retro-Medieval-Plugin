using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationKicks")]
internal class Kick() : ModerationAction(ModerationActionType.Kick)
{
}