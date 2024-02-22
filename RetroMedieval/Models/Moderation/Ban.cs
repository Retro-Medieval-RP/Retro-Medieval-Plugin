using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationBans")]
internal class Ban : ModerationAction
{
    [DatabaseColumn("BanLength", "INT", "-1")]
    public int BanLength { get; set; }
    
    [DatabaseColumn("BanOver", "TINYINT", "0")]
    public bool BanOver { get; set; }
}