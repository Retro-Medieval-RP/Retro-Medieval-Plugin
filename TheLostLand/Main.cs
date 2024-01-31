using Rocket.Core.Plugins;
using TheLostLand.Modules;

namespace TheLostLand;

internal class Main : RocketPlugin
{
    public static Main Instance { get; private set; }
    
    protected override void Load()
    {
        Instance = this;
        
        ModuleLoader.Instance.SetDirectory(Directory);
        ModuleLoader.Instance.LoadModules(Assembly);
    }

    protected override void Unload()
    {
    }
}