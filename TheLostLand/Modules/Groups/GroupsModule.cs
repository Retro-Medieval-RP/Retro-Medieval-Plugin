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
}