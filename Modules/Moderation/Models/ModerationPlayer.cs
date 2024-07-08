using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Modules;
using RetroMedieval.Savers.MySql;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Rocket.Core.Logging;

namespace Moderation.Models;

[DatabaseTable("Players")]
internal class ModerationPlayer
{
    [DatabaseColumn("PlayerID", "BIGINT")]
    [PrimaryKey]
    public ulong PlayerID { get; set; }

    [DatabaseColumn("DisplayName", "VARCHAR(255)")]
    public string DisplayName { get; set; }

    [DatabaseColumn("LastJoinDate", "DATETIME")]
    public DateTime LastJoinDate { get; set; }

    [DatabaseColumn("FirstJoinDate", "DATETIME")]
    public DateTime FirstJoinDate { get; set; }

    [DatabaseIgnore]
    public List<Ban> Bans
    {
        get
        {
            if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var module))
            {
                Logger.LogError("Could not get Module [ModerationModule]");
                return [];
            }

            if (module.GetStorage<MySqlSaver<Ban>>(out var bans))
            {
                return bans.StartQuery().Select("*").Where(("TargetID", PlayerID)).Finalise().Query<Ban>().ToList();
            }
            
            Logger.LogError("Could not get storage [BansStorage]"); 
            return [];
        }
    }

    [DatabaseIgnore]
    public List<Warn> Warns
    {
        get
        {
            if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var module))
            {
                Logger.LogError("Could not get Module [ModerationModule]");
                return [];
            }

            if (module.GetStorage<MySqlSaver<Warn>>(out var warns))
            {
                return warns.StartQuery().Select("*").Where(("TargetID", PlayerID)).Finalise().Query<Warn>().ToList();
            }
            
            Logger.LogError("Could not get storage [WarnsStorage]"); 
            return [];
        }
    }

    [DatabaseIgnore]
    public List<Kick> Kicks
    {
        get
        {
            if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var module))
            {
                Logger.LogError("Could not get Module [ModerationModule]");
                return [];
            }

            if (module.GetStorage<MySqlSaver<Kick>>(out var kicks))
            {
                return kicks.StartQuery().Select("*").Where(("TargetID", PlayerID)).Finalise().Query<Kick>().ToList();
            }
            
            Logger.LogError("Could not get storage [KicksStorage]"); 
            return [];
        }
    }

    [DatabaseIgnore]
    public List<Mute> Mutes
    {
        get
        {
            if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var module))
            {
                Logger.LogError("Could not get Module [ModerationModule]");
                return [];
            }

            if (module.GetStorage<MySqlSaver<Mute>>(out var mutes))
            {
                return mutes.StartQuery().Select("*").Where(("TargetID", PlayerID)).Finalise().Query<Mute>().ToList();
            }
            
            Logger.LogError("Could not get storage [MutesStorage]"); 
            return [];
        }
    }
}