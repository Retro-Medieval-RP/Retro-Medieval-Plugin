using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Models.Moderation;

internal class ModerationAction(ModerationActionType actionType)
{
    [DatabaseIgnore] 
    public ModerationActionType ModerationActionType { get; set; } = actionType;
    
    [DatabaseColumn("PunishmentID", "CHAR(36)")]
    [PrimaryKey]
    public Guid PunishmentID { get; set; }

    [DatabaseColumn("PunisherID", "BIGINT")] 
    public ulong PunisherID { get; set; }

    [DatabaseColumn("TargetID", "BIGINT")] 
    public ulong TargetID { get; set; }
    
    [DatabaseColumn("PunishmentGiven", "DATETIME", "")]
    public DateTime PunishmentGiven { get; set; }

    [DatabaseColumn("Reason", "VARCHAR(255)")]
    public string Reason { get; set; } = "No Reason Specified";
    
    public static int? ConvertToBanDuration(IEnumerable<string> args)
    {
        int? seconds = 0;

        var enumerable = args as string[] ?? args.ToArray();
        if (!enumerable.Any())
        {
            return seconds == 0 ? null : seconds;
        }
        
        var timePeriods = new Dictionary<char, int>()
        {
            { 'd', 86400 },
            { 'h', 3600 },
            { 'm', 60 },
            { 's', 1 }
        };


        foreach (var arg in enumerable)
        {
            foreach (var pair in timePeriods.Where(pair => arg.Contains(pair.Key)))
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