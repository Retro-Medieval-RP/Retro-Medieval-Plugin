using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Moderation.Models;

[DatabaseTable("ModerationBans")]
internal class Ban() : ModerationAction(ModerationActionType.Ban)
{
    [DatabaseColumn("BanLength", "INT", "-1")]
    public int BanLength { get; set; } = -1;
    
    [DatabaseColumn("BanOver", "TINYINT", "0")]
    public bool BanOver { get; set; }
    
    [DatabaseIgnore]
    public bool IsExpired 
    {
        get
        {
            if (BanLength != -1)
                return DateTime.Now > PunishmentGiven.AddSeconds(BanLength);
            return false;
        }
    }

    [DatabaseIgnore]
    public string DurationString
    {
        get
        {
            if (BanLength == -1)
            {
                return "Ban is permanent";
            }
            
            var span = TimeSpan.FromSeconds(BanLength);
            return TimeSpanString(span);
        }
    }
    
    [DatabaseIgnore]
    public string TimeLeftString
    {
        get
        {
            if (BanLength == -1)
            {
                return "Permanent";
            }
            
            var span = PunishmentGiven.AddSeconds(BanLength) - DateTime.Now;
            return TimeSpanString(span);
        }
    }

    private static string TimeSpanString(TimeSpan span)
    {
        var formatted =
            $"{(span.Duration().Days > 0 ? $"{span.Days:0}d " : string.Empty)}{(span.Duration().Hours > 0 ? $"{span.Hours:0}h " : string.Empty)}{(span.Duration().Minutes > 0 ? $"{span.Minutes:0}m " : string.Empty)}";

        if (formatted.EndsWith(" ")) formatted = formatted.Substring(0, formatted.Length - 1);
        if (string.IsNullOrEmpty(formatted)) formatted = "<1m";

        return formatted;
    }
}