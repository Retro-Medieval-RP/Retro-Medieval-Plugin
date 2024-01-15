using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Core.Utils;

namespace TheLostLand.Core.Modules;

public sealed class ModuleLoader : Padlock<ModuleLoader>
{
    internal string ModuleDirectory { get; private set; }

    private List<Module> Modules { get; set; } = [];
    
    public void SetDirectory(string dir) => ModuleDirectory = dir;

    public void LoadModules(Assembly plugin)
    {
        var modules = plugin.GetTypes()
            .Where(x => x.BaseType == typeof(Module));

        foreach (var m in modules)
        {
            var module = Activator.CreateInstance(m) as Module;
            var ats = module.GetType().GetCustomAttributes(false);
            Modules.Add(module);
        }
    }
}