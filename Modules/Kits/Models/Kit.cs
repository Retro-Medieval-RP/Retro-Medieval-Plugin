using System;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace Kits.Models;


[DatabaseTable("Kits")]
internal class Kit
{
    [DatabaseColumn("KitID", "CHAR(36)")]
    [PrimaryKey]
    public Guid KitID { get; set; }

    [DatabaseColumn("KitName", "VARCHAR(255)")]
    public string KitName { get; set; }

    [DatabaseColumn("KitCooldown", "INT", "-1")]
    public int KitCooldown { get; set; }
    
    [DatabaseIgnore]
    public string CooldownString
    {
        get
        {
            if (KitCooldown == 0)
            {
                return "0s";
            }
            
            return TimeSpanString(TimeSpan.FromSeconds(KitCooldown));
        }
    }

    internal string TimeSpanString(TimeSpan span)
    {
        var formatted =
            $"{(span.Duration().Days > 0 ? $"{span.Days:0}d " : string.Empty)}{(span.Duration().Hours > 0 ? $"{span.Hours:0}h " : string.Empty)}{(span.Duration().Minutes > 0 ? $"{span.Minutes:0}m" : string.Empty)}";

        if (formatted.EndsWith(" ")) formatted = formatted.Substring(0, formatted.Length - 1);
        if (string.IsNullOrEmpty(formatted)) formatted = "<1m";

        return formatted;
    }
}