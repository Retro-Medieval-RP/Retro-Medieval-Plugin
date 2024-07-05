using System.IO;
using System.Timers;
using HarmonyLib;
using RetroMedieval.Modules;
using Rocket.Core.Plugins;

namespace RetroMedieval;

public class Main : RocketPlugin
{
    public static Main Instance { get; private set; }
    
    protected override void Load()
    {
        Instance = this;
        
        ModuleLoader.Instance.SetDirectory(Directory);

        var files = System.IO.Directory.GetFiles("./Plugins/RetroMedieval/Modules", "*.dll", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var assemblyBytes = File.ReadAllBytes(file);
            var assembly = System.Reflection.Assembly.Load(assemblyBytes);
            ModuleLoader.Instance.LoadModules(assembly);
        }
        
        ModuleLoader.Instance.SetUpdateTimer(new Timer(10000));
        
        var harmony = new Harmony("com.retromedieval.patch");
        harmony.PatchAll();
    }

    protected override void Unload()
    {
    }
}