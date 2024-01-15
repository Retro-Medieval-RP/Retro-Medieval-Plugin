using Rocket.Core.Plugins;
using TheLostLand.Core.Modules;

namespace TheLostLand;

internal class Main : RocketPlugin
{
    protected override void Load()
    {
        ModuleLoader.Instance.SetDirectory(Directory);
        ModuleLoader.Instance.LoadModules(Assembly);
    }

    protected override void Unload()
    {
    }
}