using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

internal class ModerationAction(ModerationActionType action_type)
{
    [DatabaseIgnore] 
    public ModerationActionType ModerationActionType { get; set; } = action_type;
    
    [DatabaseColumn("PunishmentID", "CHAR(36)")]
    [PrimaryKey]
    public Guid PunishmentID { get; set; }

    [DatabaseColumn("PunisherID", "")] 
    public ulong PunisherID { get; set; }

    [DatabaseColumn("TargetID", "")] 
    public ulong TargetID { get; set; }
    
    [DatabaseColumn("PunishmentGiven", "DATETIME", "")]
    public DateTime PunishmentGiven { get; set; }

    [DatabaseColumn("Reason", "VARCHAR(255)", "No Reason Specified")]
    public string Reason { get; set; }
    
    public static int? ConvertToBanDuration(IEnumerable<string> args)
    {
        int? seconds = 0;

        var enumerable = args as string[] ?? args.ToArray();
        if (!enumerable.Any())
        {
            return seconds == 0 ? null : seconds;
        }
        
        var time_periods = new Dictionary<char, int>()
        {
            { 'd', 86400 },
            { 'h', 3600 },
            { 'm', 60 },
            { 's', 1 }
        };


        foreach (var arg in enumerable)
        {
            foreach (var pair in time_periods.Where(pair => arg.Contains(pair.Key)))
            {
                if (int.TryParse(arg.Trim(pair.Key), out var result))
                {
                    seconds += result * pair.Value;
                }
                break;
            }
        }

        return seconds == 0 ? null : seconds;
    }
}