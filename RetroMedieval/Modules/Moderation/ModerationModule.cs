using RetroMedieval.Models.Moderation;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Savers.MySql;

namespace RetroMedieval.Modules.Moderation;

[ModuleInformation("Moderation")]
[ModuleStorage<MySqlSaver<Warn>>("Warns")]
[ModuleStorage<MySqlSaver<Mute>>("Mutes")]
[ModuleStorage<MySqlSaver<Kick>>("Kicks")]
[ModuleStorage<MySqlSaver<Ban>>("Bans")]
internal class ModerationModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }
}