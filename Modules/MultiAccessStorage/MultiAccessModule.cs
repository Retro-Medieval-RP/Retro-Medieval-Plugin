using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;

namespace MultiAccessStorage;

[ModuleInformation("Multi User Access Storage")]
public class MultiAccessModule(string directory) : Module(directory)
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }
}