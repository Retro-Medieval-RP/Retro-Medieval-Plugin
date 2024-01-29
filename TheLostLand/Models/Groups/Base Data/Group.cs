using System.Collections.Generic;

namespace TheLostLand.Models.Groups.Base_Data;

public class Group
{
    public string GroupName { get; set; }
    public List<GroupMember> GroupMembers { get; set; }
    public List<GroupRank> GroupRanks { get; set; }
    public List<BaseLocation> BaseLocations { get; set; }
}