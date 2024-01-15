using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TheLostLand.Core.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleInformation : Attribute
{
    private string ModuleName { get; }
    private List<ModuleConfiguration> Configs { get; }
    private List<ModuleStorage> Storages { get; }

    public ModuleInformation(string module_name)
    {
        ModuleName = module_name;
        Configs = [];
        Storages = [];

        LoadStorages();
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        var configs = GetType().GetCustomAttributes<ModuleConfiguration>();

        foreach (var config in configs)
        {
            if (config.LoadedConfiguration(Path.Combine(ModuleLoader.Instance.ModuleDirectory, ModuleName), config.Name + ".json"))
            {
                Configs.Add(config);
            }
            
            Console.WriteLine("Failed To Load Config: " + config.Name);
        }
    }

    private void LoadStorages()
    {
        var configs = GetType().GetCustomAttributes<ModuleStorage>();

        foreach (var storage in configs)
        {
            if (storage.LoadedStorage(Path.Combine(ModuleLoader.Instance.ModuleDirectory, ModuleName), storage.Name + ".json"))
            {
                Storages.Add(storage);
            }
            
            Console.WriteLine("Failed To Load Storage: " + storage.Name);
        }
    }
}