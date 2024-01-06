using System.Reflection;
using Rocket.Core.Logging;

namespace TheLostLand.Modules;

public class ModuleLoader : Padlock<ModuleLoader>
{
    private readonly List<Module> _modulesLoaded = [];

    public Module this[string module_name] =>
        _modulesLoaded.FirstOrDefault(x => x.ModuleInformation.ModuleName == module_name);

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
}