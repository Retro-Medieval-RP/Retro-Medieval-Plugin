using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using RetroMedieval.Modules.Configuration;
using RetroMedieval.Modules.Storage;
using RetroMedieval.Utils;

namespace RetroMedieval.Modules;

public sealed class ModuleLoader : Padlock<ModuleLoader>
{
    internal string ModuleDirectory { get; private set; } = "";

    private List<Module> Modules { get; } = [];
    
    private Timer? Timer { get; set; }

    ~ModuleLoader()
    {
        if (Timer != null)
        {
            Timer.Elapsed -= OnTimerElapsed;
        }
        
        Timer = null;
    }
    
    public bool GetModule<TModule>(out TModule module) where TModule : class
    {
        if (Modules.All(x => x.GetType() != typeof(TModule)))
        {
            module = default!;
            return false;
        }

        module = (Modules.Find(x => x.GetType() == typeof(TModule)) as TModule)!;
        return true;
    }
    
    public void SetDirectory(string dir) => ModuleDirectory = dir;

    public void LoadModules(Assembly plugin)
    {
        var modules = plugin.GetTypes()
            .Where(x => x.BaseType == typeof(Module));

        foreach (var m in modules)
        {
            var module = Activator.CreateInstance(m, ModuleDirectory) as Module;
            module?.Load();

            if (module != null)
            {
                Modules.Add(module);
            }
        }
    }

    public void ReloadAllModules(Assembly plugin)
    {
        ConfigurationManager.Instance.Clear();
        StorageManager.Instance.Clear();

        foreach (var m in Modules)
        {
            m.Unload();
        }
        
        Modules.Clear();
        
        LoadModules(plugin);
    }

    public bool Exists(string moduleName) => 
        Modules.Any(x => x.NameIs(moduleName));

    public void SetUpdateTimer(Timer timer)
    {
        Timer = timer;
        Timer.Elapsed += OnTimerElapsed;
        Timer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        foreach (var m in Modules) m.CallTick();
    }
}