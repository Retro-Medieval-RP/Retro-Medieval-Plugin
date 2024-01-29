using TheLostLand.Modules.Attributes;

namespace TheLostLand.Modules.Groups;

[ModuleInformation("Groups")]
[ModuleStorage<GuildGroupStorage>("Guilds")]
[ModuleStorage<KingdomsGroupStorage>("Kingdoms")]
public class GroupsModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }
}