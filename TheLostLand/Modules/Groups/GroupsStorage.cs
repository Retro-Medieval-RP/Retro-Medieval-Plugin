using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
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
                    PermissionLevels =
                    [
                        PermissionLevel.Ban,
                        PermissionLevel.Kick,
                        PermissionLevel.Unban,
                        PermissionLevel.RejectUsers,
                        PermissionLevel.AcceptUsers
                    ],
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

    public bool HasPermission(ulong steam_id, PermissionLevel permission)
    {
        if (!StorageItem.Any(x => x.GroupMembers.Any(group_member => group_member.UserID == steam_id)))
        {
            return false;
        }

        var group = StorageItem.Find(x => x.GroupMembers.Any(group_member => group_member.UserID == steam_id));
        var member = group.GroupMembers.Find(x => x.UserID == steam_id);
        var rank = group.GroupRanks.Find(x => x.RankID == member.RankID);

        return rank.PermissionLevels.Contains(permission);
    }

    public bool AnyMembers(string group_name)
    {
        if (StorageItem.All(x => x.GroupName != group_name))
        {
            return false;
        }

        return StorageItem.Find(x => x.GroupName == group_name).GroupMembers.Count != 0;
    }

    public bool Join(ulong player_id, string group_name)
    {
        if (StorageItem.All(x => x.GroupName != group_name))
        {
            return false;
        }

        if (StorageItem.All(x => x.GroupMembers.Any(group_member => group_member.UserID == player_id)))
        {
            if (!Leave(player_id))
            {
                return false;
            }
        }

        var new_group = StorageItem.Find(x => x.GroupName == group_name);
        new_group.GroupMembers.Add(new GroupMember
        {
            UserID = player_id,
            RankID = new_group.DefaultRankId
        });

        Save();
        return true;
    }

    public bool Leave(ulong player_id)
    {
        if (!StorageItem.All(x => x.GroupMembers.Any(group_member => group_member.UserID == player_id)))
        {
            return false;
        }

        var group = StorageItem.Find(x => x.GroupMembers.Any(group_member => group_member.UserID == player_id));
        group.GroupMembers.RemoveAll(x => x.UserID == player_id);
        
        Save();
        return true;
    }

    public bool Kick(ulong moderator, string player)
    {
        if (ulong.TryParse(player, out var player_id))
        {
            return Leave(player_id);
        }
        
        var group = StorageItem.Find(x => x.GroupMembers.Any(x => x.UserID == player_id));
        if (group.GroupMembers.All(x => x.UserID != moderator))
        {
            return false;
        }
        
        var p = UnturnedPlayer.FromName(player);
        player_id = p.CSteamID.m_SteamID;

        return Leave(player_id);
    }

    public bool Ban(ulong banner, string player)
    {
        if (ulong.TryParse(player, out var player_id))
        {
            var p = UnturnedPlayer.FromName(player);
            player_id = p.CSteamID.m_SteamID;
        }

        var group = StorageItem.Find(x => x.GroupMembers.Any(x => x.UserID == player_id));

        if (group.GroupMembers.All(x => x.UserID != banner))
        {
            return false;
        }
        
        if (!Leave(player_id))
        {
            return false;
        }
        
        group.BannedPlayers.Add(player_id);
        Save();
        return true;
    }

    public bool Unban(ulong moderator, string player)
    {
        if (ulong.TryParse(player, out var player_id))
        {
            var p = UnturnedPlayer.FromName(player);
            player_id = p.CSteamID.m_SteamID;
        }
        
        var group = StorageItem.Find(x => x.GroupMembers.Any(group_member => group_member.UserID == player_id));
        if (group.GroupMembers.All(x => x.UserID != moderator))
        {
            return false;
        }

        if (!group.BannedPlayers.Contains(player_id))
        {
            return false;
        }
        
        group.BannedPlayers.Remove(player_id);
        Save();
        return true;
    }

    public void Delete(string group_name)
    {
    }
}