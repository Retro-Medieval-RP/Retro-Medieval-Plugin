using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheLostLand.Core.Utils;

namespace TheLostLand.Core.Modules;

public class ModuleLoader : Padlock<ModuleLoader>
{
    private readonly List<Module> _modulesLoaded = [];
    
    internal string SaveDirectory { get; private set; }

    public void Load(Assembly assembly, string dir)
    {
        SaveDirectory = dir;
        
        var modules = assembly.GetTypes()
            .Where(x => x.BaseType == typeof(Module))
            .Select(Activator.CreateInstance)
            .Select(x => x as Module);

        foreach (var module in modules)
        {
            Console.WriteLine("Found Module: " + module?.ModuleInformation.ModuleName);
            _modulesLoaded.Add(module);
        }
    }

    public bool GetModule<TModule>(out TModule module) where TModule : Module
    {
        if (_modulesLoaded.Exists(x => x.ModuleType(typeof(TModule))))
        {
            module = _modulesLoaded.Find(x => x.ModuleType(typeof(TModule))) as TModule;
            return true;
        }

        module = default;
        return false;
    }
}