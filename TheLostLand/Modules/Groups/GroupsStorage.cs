using System.Collections.Generic;
using System.Linq;
using TheLostLand.Models.Groups;
using TheLostLand.Savers;

namespace TheLostLand.Modules.Groups;

public class GroupsStorage : JsonSaver<List<Group>>
{
    public bool CreateGroup(string group_name, ulong player_id)
    {
        var new_group = new Group
        {
            GroupName = group_name,
            BaseLocations = [],
            GroupMembers =
            [
                new GroupMember
                {
                    RankID = 0,
                    UserID = player_id
                }
            ],
            GroupRanks =
            [
                new GroupRank
                {
                    UnbanUsers = true,
                    AcceptUsers = true,
                    BanUsers = true,
                    KickUsers = true,
                    RankID = 0,
                    RankName = "GroupOwner",
                }
            ]
        };
        
        StorageItem.Add(new_group);
        Save();
        
        return true;
    }

    public bool Exists(string group_name) => 
        StorageItem.Any(x => x.GroupName == group_name);
}