using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

[DatabaseTable("ModerationMutes")]
internal class Mute : ModerationAction
{
    [DatabaseColumn("MuteLength", "INT", "-1")]
    public int MuteLength { get; set; }
    
    [DatabaseColumn("BanOver", "TINYINT", "0")]
    public bool MuteOver { get; set; }
    
    [DatabaseIgnore]
    public bool IsExpired 
    {
        get 
        {
            if (MuteLength != -1)
                return DateTime.Now > PunishmentGiven.AddSeconds(MuteLength);
            else
                return false;
        }
    }

    [DatabaseIgnore]
    public string DurationString
    {
        get
        {
            if (MuteLength == -1)
            {
                return "Ban is permanent";
            }
            
            var span = TimeSpan.FromSeconds(MuteLength);
            return TimeSpanString(span);
        }
    }
    
    [DatabaseIgnore]
    public string TimeLeftString
    {
        get
        {
            if (MuteLength == -1)
            {
                return "Permanent";
            }
            
            var span = PunishmentGiven.AddSeconds(MuteLength) - DateTime.Now;
            return TimeSpanString(span);
        }
    }

    private string TimeSpanString(TimeSpan span)
    {
        var formatted =
            $"{(span.Duration().Days > 0 ? $"{span.Days:0}d " : string.Empty)}{(span.Duration().Hours > 0 ? $"{span.Hours:0}h " : string.Empty)}{(span.Duration().Minutes > 0 ? $"{span.Minutes:0}m " : string.Empty)}";

        if (formatted.EndsWith(" ")) formatted = formatted.Substring(0, formatted.Length - 1);
        if (string.IsNullOrEmpty(formatted)) formatted = "<1m";

        return formatted;
    }
}