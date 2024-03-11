using System.Timers;
using HarmonyLib;
using RetroMedieval.Modules;
using Rocket.Core.Plugins;

namespace RetroMedieval;

internal class Main : RocketPlugin
{
    public static Main Instance { get; private set; }
    
    protected override void Load()
    {
        Instance = this;
        
        ModuleLoader.Instance.SetDirectory(Directory);
        ModuleLoader.Instance.LoadModules(Assembly);
        ModuleLoader.Instance.SetUpdateTimer(new Timer(10000));
        
        var harmony = new Harmony("com.retromedieval.patch");
        harmony.PatchAll();
    }

    protected override void Unload()
    {
    }
}