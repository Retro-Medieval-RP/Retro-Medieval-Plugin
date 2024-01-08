using System.Reflection;
using Rocket.Core.Logging;

namespace TheLostLand.Modules;

public class ModuleLoader : Padlock<ModuleLoader>
{
    private readonly List<Module> _modulesLoaded = [];
    
    internal void Load(Assembly assembly)
    {
        var modules = assembly.GetTypes()
            .Where(x => x.BaseType == typeof(Module))
            .Select(Activator.CreateInstance)
            .Select(x => x as Module);

        foreach (var module in modules)
        {
            Logger.Log("Found Module: " + module?.ModuleInformation.ModuleName);
            _modulesLoaded.Add(module);
        }
    }

    internal bool GetModule<TModule>(out TModule module) where TModule : Module
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