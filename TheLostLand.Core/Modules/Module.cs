using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.Core.Logging;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Core.Modules.Configuration;
using TheLostLand.Core.Modules.Storage;

namespace TheLostLand.Core.Modules;

public abstract class Module
{
    private ModuleInformation Information => GetType().GetCustomAttribute<ModuleInformation>();
    private List<ModuleConfiguration> Configurations { get; set; } = [];
    private List<ModuleStorage> Storages { get; set; } = [];

    protected Module()
    {
        LoadConfigs();
        LoadStorages();
    }

    public abstract void Load();
    public abstract void Unload();
    
    private void LoadConfigs()
    {
        var configs = GetType().GetCustomAttributes<ModuleConfiguration>();

        foreach (var config in configs)
        {
            if (config.LoadedConfiguration(Path.Combine(ModuleLoader.Instance.ModuleDirectory, Information.ModuleName), config.Name + ".json"))
            {
                Configurations.Add(config);
                Logger.Log("Successfully Loaded Config: " + config.Name);
                continue;
            }
            
            Logger.Log("Failed To Load Config: " + config.Name);
        }
    }

    private void LoadStorages()
    {
        var configs = GetType().GetCustomAttributes<ModuleStorage>();

        foreach (var storage in configs)
        {
            if (storage.LoadedStorage(Path.Combine(ModuleLoader.Instance.ModuleDirectory, Information.ModuleName), storage.Name + ".json"))
            {
                Storages.Add(storage);
                Logger.Log("Successfully Loaded Storage: " + storage.Name);
                continue;
            }
            
            Logger.Log("Failed To Load Storage: " + storage.Name);
        }
    }
    
    protected bool GetConfiguration<TConfiguration>(out TConfiguration config) where TConfiguration : class, IConfig, new()
    {
        if (Configurations.Any(x => x.IsConfigOfType(typeof(TConfiguration))))
        {
            config = GetType().GetCustomAttribute<ModuleConfiguration<TConfiguration>>().Configuration;
            return true;
        }

        config = default;
        return false;
    }

    protected bool GetStorage<TStorage>(out TStorage storage) where TStorage : class, IStorage, new()
    {
        if (Storages.Any(x => x.IsStorageOfType(typeof(TStorage))))
        {
            storage = GetType().GetCustomAttribute<ModuleStorage<TStorage>>().Storage;
            return true;
        }

        storage = default;
        return false;
    }
}