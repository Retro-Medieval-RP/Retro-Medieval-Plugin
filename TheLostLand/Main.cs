using HarmonyLib;
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
        
        var harmony = new Harmony("com.thelostland.patch");
        harmony.PatchAll();
    }

    protected override void Unload()
    {
    }
}