using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationMutes")]
internal class Mute : ModerationAction
{
    [DatabaseColumn("BanLength", "INT", "-1")]
    public int MuteLength { get; set; }
    
    [DatabaseColumn("BanOver", "TINYINT", "0")]
    public bool MuteOver { get; set; }
}