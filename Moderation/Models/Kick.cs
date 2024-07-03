using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Moderation.Models;

[DatabaseTable("ModerationKicks")]
internal class Kick() : ModerationAction(ModerationActionType.Kick)
{
}