﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheLostLand.Modules.Configuration;
using TheLostLand.Modules.Storage;
using TheLostLand.Utils;

namespace TheLostLand.Modules;

public sealed class ModuleLoader : Padlock<ModuleLoader>
{
    internal string ModuleDirectory { get; private set; }

    private List<Module> Modules { get; } = [];

    public bool GetModule<TModule>(out TModule module) where TModule : class
    {
        if (Modules.All(x => x.GetType() != typeof(TModule)))
        {
            module = default;
            return false;
        }

        module = Modules.Find(x => x.GetType() == typeof(TModule)) as TModule;
        return true;
    }
    
    public void SetDirectory(string dir) => ModuleDirectory = dir;

    public void LoadModules(Assembly plugin)
    {
        var modules = plugin.GetTypes()
            .Where(x => x.BaseType == typeof(Module));

        foreach (var m in modules)
        {
            var module = Activator.CreateInstance(m) as Module;
            module?.Load();
            
            Modules.Add(module);
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

    public bool Exists(string module_name) => 
        Modules.Any(x => x.NameIs(module_name));
}