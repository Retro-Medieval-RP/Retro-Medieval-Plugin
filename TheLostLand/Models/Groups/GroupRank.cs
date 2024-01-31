using System.Collections.Generic;

namespace TheLostLand.Models.Groups;

public class GroupRank
{
    public int RankID { get; set; }
    public string RankName { get; set; }
    
    public List<PermissionLevel> PermissionLevels { get; set; }
}