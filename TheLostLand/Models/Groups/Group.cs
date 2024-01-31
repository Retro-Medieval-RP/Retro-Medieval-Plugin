using System.Collections.Generic;

namespace TheLostLand.Models.Groups;

public class Group
{
    public string GroupName { get; set; }
    public int DefaultRankId { get; set; }
    public List<GroupMember> GroupMembers { get; set; }
    public List<GroupRank> GroupRanks { get; set; }
    public List<BaseLocation> BaseLocations { get; set; }
    
    public List<ulong> BannedPlayers { get; set; }
}