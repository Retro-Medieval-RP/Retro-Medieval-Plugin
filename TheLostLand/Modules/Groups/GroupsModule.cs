using Rocket.Unturned.Player;
using TheLostLand.Models.Groups;
using TheLostLand.Modules.Attributes;

namespace TheLostLand.Modules.Groups;

[ModuleInformation("Groups")]
[ModuleStorage<GroupsStorage>("Groups")]
public class GroupsModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public bool CreateGroup(string group_name, ulong player_id) => 
        GetStorage<GroupsStorage>(out var storage) && storage.CreateGroup(group_name, player_id);

    public bool Exists(string group_name) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Exists(group_name);

    public bool HasPermission(UnturnedPlayer caller, PermissionLevel permission) => 
        GetStorage<GroupsStorage>(out var storage) && storage.HasPermission(caller.CSteamID.m_SteamID, permission);

    public bool Unban(ulong m_steam_id, string player) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Unban(m_steam_id, player);

    public bool Ban(ulong m_steam_id, string player) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Ban(m_steam_id, player);

    public bool Kick(ulong m_steam_id, string player) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Kick(m_steam_id, player);

    public bool LeaveGroup(UnturnedPlayer caller) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Leave(caller.CSteamID.m_SteamID);

    public bool JoinGroup(UnturnedPlayer caller, string group_name) => 
        GetStorage<GroupsStorage>(out var storage) && storage.Join(caller.CSteamID.m_SteamID, group_name);

    public bool AnyMembers(string group_name) => 
        GetStorage<GroupsStorage>(out var storage) && storage.AnyMembers(group_name);

    public void DeleteGroup(string group_name)
    {
        if (!GetStorage<GroupsStorage>(out var storage))
        {
            return;
        }

        storage.Delete(group_name);
    }
}